using HR.DataAccess;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.Facade.Implementation
{
    // Permission groups and user permissions (tables: Permission, PermissionGroup, PermissionGroupMap, UserPermission).
    public class PermissionFacade : IPermissionFacade
    {
        public const int CompanyId = 1;
        private readonly ApplicationDbContext _db;

        public PermissionFacade(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SyncAsync(IReadOnlyList<PermissionDef> catalog)
        {
            var existing = await _db.Permissions.ToDictionaryAsync(p => p.Name);
            var nextId = existing.Count == 0 ? 1 : existing.Values.Max(p => p.PermissionId) + 1;

            // Parents come before children in the catalog, so a single pass is enough.
            foreach (var def in catalog)
            {
                if (!existing.TryGetValue(def.Name, out var p))
                {
                    p = new Permission { PermissionId = nextId++, CompanyId = CompanyId, Name = def.Name };
                    _db.Permissions.Add(p);
                    existing[def.Name] = p;
                }
                p.DisplayText = def.DisplayText;
                p.ParentId = def.ParentName != null && existing.TryGetValue(def.ParentName, out var parent) ? parent.PermissionId : null;
                p.IsActive = true;
            }
            // Screens removed from the menu stay in the table but are switched off.
            var names = catalog.Select(c => c.Name).ToHashSet();
            foreach (var p in existing.Values.Where(p => !names.Contains(p.Name))) p.IsActive = false;
            await _db.SaveChangesAsync();

            if (!await _db.PermissionGroups.AnyAsync())
            {
                await AddDefaultGroup("Leave Officer", "Attendance and leave only", existing.Values, n => n.StartsWith("Attendance") || n.StartsWith("LeaveRequest") || n.StartsWith("LeaveBalance") || n.StartsWith("Holiday") || n == "Registers.View");
                await AddDefaultGroup("Recruitment Officer", "Jobs, candidates, interviews", existing.Values, n => n.StartsWith("JobOpening") || n.StartsWith("Candidate") || n.StartsWith("JobApplication") || n.StartsWith("Interview") || n.StartsWith("OfferLetter"));
                await AddDefaultGroup("Payroll Officer", "Payroll, bonus, loans", existing.Values, n => n.StartsWith("Payroll") || n.StartsWith("Bonus") || n.StartsWith("Loan") || n == "Reports.View");
                await AddDefaultGroup("Read Only", "Can open every screen but cannot change anything", existing.Values, n => n.EndsWith(".View"));
            }
        }

        private async Task AddDefaultGroup(string name, string tag, IEnumerable<Permission> all, Func<string, bool> match)
        {
            var g = new PermissionGroup { CompanyId = CompanyId, Name = name, Tag = tag };
            foreach (var p in all.Where(p => p.Name.Contains('.') && match(p.Name)))
                g.Maps.Add(new PermissionGroupMap { PermissionId = p.PermissionId, CompanyId = CompanyId });
            _db.PermissionGroups.Add(g);
            await _db.SaveChangesAsync();
        }

        public async Task<HashSet<string>?> GetUserPermissionNamesAsync(int userId)
        {
            var rows = await _db.UserPermissions.Where(u => u.UserId == userId).ToListAsync();
            if (rows.Count == 0) return null;

            var groupIds = rows.Where(r => r.PermissionGroupId != null).Select(r => r.PermissionGroupId!.Value).ToList();
            var direct = rows.Where(r => r.PermissionId != null).Select(r => r.PermissionId!.Value).ToList();
            var fromGroups = await _db.PermissionGroupMaps.Where(m => groupIds.Contains(m.PermissionGroupId) && m.IsActive).Select(m => m.PermissionId).ToListAsync();
            var ids = direct.Concat(fromGroups).Distinct().ToList();
            return (await _db.Permissions.Where(p => ids.Contains(p.PermissionId) && p.IsActive).Select(p => p.Name).ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public Task<List<Permission>> GetAllPermissionsAsync() =>
            _db.Permissions.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.PermissionId).ToListAsync();

        public Task<List<PermissionGroupSummary>> GetGroupsAsync() =>
            _db.PermissionGroups.OrderBy(g => g.Name)
                .Select(g => new PermissionGroupSummary(g.PermissionGroupId, g.Name, g.Tag, g.Maps.Count(m => m.IsActive), g.UserPermissions.Count))
                .ToListAsync();

        public async Task<(PermissionGroup Group, HashSet<int> PermissionIds)> GetGroupAsync(int id)
        {
            var g = await _db.PermissionGroups.AsNoTracking().FirstOrDefaultAsync(x => x.PermissionGroupId == id)
                    ?? throw new BusinessException("Permission group not found.");
            var ids = await _db.PermissionGroupMaps.Where(m => m.PermissionGroupId == id && m.IsActive).Select(m => m.PermissionId).ToListAsync();
            return (g, ids.ToHashSet());
        }

        public async Task<int> SaveGroupAsync(int id, string name, string? tag, IEnumerable<int> permissionIds)
        {
            name = (name ?? "").Trim();
            if (name.Length == 0) throw new BusinessException("Group name is required.");
            if (name.Length > 50) throw new BusinessException("Group name can be at most 50 characters.");
            if (await _db.PermissionGroups.AnyAsync(g => g.Name == name && g.PermissionGroupId != id))
                throw new BusinessException($"A group named '{name}' already exists.");

            var g = id == 0 ? _db.PermissionGroups.Add(new PermissionGroup { CompanyId = CompanyId }).Entity
                : await _db.PermissionGroups.Include(x => x.Maps).FirstOrDefaultAsync(x => x.PermissionGroupId == id) ?? throw new BusinessException("Permission group not found.");
            g.Name = name;
            g.Tag = string.IsNullOrWhiteSpace(tag) ? null : tag.Trim();

            var wanted = await ValidIds(permissionIds);
            foreach (var m in g.Maps.ToList())
            {
                if (wanted.Contains(m.PermissionId)) m.IsActive = true;
                else _db.PermissionGroupMaps.Remove(m);
            }
            foreach (var pid in wanted.Where(p => g.Maps.All(m => m.PermissionId != p)))
                g.Maps.Add(new PermissionGroupMap { PermissionId = pid, CompanyId = CompanyId });

            await _db.SaveChangesAsync();
            return g.PermissionGroupId;
        }

        public async Task DeleteGroupAsync(int id)
        {
            var g = await _db.PermissionGroups.Include(x => x.Maps).FirstOrDefaultAsync(x => x.PermissionGroupId == id)
                    ?? throw new BusinessException("Permission group not found.");
            if (await _db.UserPermissions.AnyAsync(u => u.PermissionGroupId == id))
                throw new BusinessException("This group is given to one or more users. Remove it from those users first.");
            _db.PermissionGroupMaps.RemoveRange(g.Maps);
            _db.PermissionGroups.Remove(g);
            await _db.SaveChangesAsync();
        }

        public async Task<(HashSet<int> GroupIds, HashSet<int> PermissionIds)> GetUserAssignmentAsync(int userId)
        {
            var rows = await _db.UserPermissions.AsNoTracking().Where(u => u.UserId == userId).ToListAsync();
            return (rows.Where(r => r.PermissionGroupId != null).Select(r => r.PermissionGroupId!.Value).ToHashSet(),
                    rows.Where(r => r.PermissionId != null).Select(r => r.PermissionId!.Value).ToHashSet());
        }

        public async Task SaveUserAssignmentAsync(int userId, IEnumerable<int> groupIds, IEnumerable<int> permissionIds)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new BusinessException("User not found.");
            if (user.RoleId == RoleIds.Admin) throw new BusinessException("Admin always has every permission; nothing to assign.");

            var groups = groupIds.Distinct().ToList();
            var validGroups = await _db.PermissionGroups.Where(g => groups.Contains(g.PermissionGroupId)).Select(g => g.PermissionGroupId).ToListAsync();
            var perms = await ValidIds(permissionIds);

            _db.UserPermissions.RemoveRange(_db.UserPermissions.Where(u => u.UserId == userId));
            foreach (var g in validGroups) _db.UserPermissions.Add(new UserPermission { CompanyId = CompanyId, UserId = userId, PermissionGroupId = g });
            foreach (var p in perms) _db.UserPermissions.Add(new UserPermission { CompanyId = CompanyId, UserId = userId, PermissionId = p });
            await _db.SaveChangesAsync();
        }

        private async Task<List<int>> ValidIds(IEnumerable<int> ids)
        {
            var list = ids.Distinct().ToList();
            return await _db.Permissions.Where(p => list.Contains(p.PermissionId) && p.IsActive).Select(p => p.PermissionId).ToListAsync();
        }
    }
}
