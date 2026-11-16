using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using HR.DataAccess.Interface;
using HR.DataAccess.Reporting;
using HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HR.DataAccess.Implementation
{
    // Generic read access to any table, driven by the EF Core model:
    // display names for dropdowns, one record with its parent links and all child tables,
    // and a summary of all tables with their relations.
    public class RecordRepository : IRecordRepository
    {
        private static readonly string[] HiddenColumns = { "IsDeleted", "DeletedAt", "PasswordHash", "ResumePath" };
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<Type, Dictionary<int, string>> _lookupCache = new();

        public RecordRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Type? FindType(string entityName) =>
            _db.Model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))?.ClrType;

        // ---------------- Lookups (id -> display text) ----------------

        public async Task<Dictionary<int, string>> GetLookupAsync(Type type)
        {
            if (_lookupCache.TryGetValue(type, out var cached)) return cached;
            var result = await BuildLookupAsync(type, 0);
            _lookupCache[type] = result;
            return result;
        }

        private async Task<Dictionary<int, string>> BuildLookupAsync(Type type, int depth)
        {
            switch (type.Name)
            {
                case nameof(Employee):
                    return await _db.Employees.AsNoTracking()
                        .Select(e => new { e.EmployeeId, Text = e.EmployeeCode + " - " + e.User.FullName })
                        .ToDictionaryAsync(x => x.EmployeeId, x => x.Text);
                case nameof(User):
                    return await _db.Users.AsNoTracking()
                        .Select(u => new { u.UserId, Text = u.FullName + " (" + u.Email + ")" })
                        .ToDictionaryAsync(x => x.UserId, x => x.Text);
                case nameof(Role):
                    return await _db.Roles.AsNoTracking().ToDictionaryAsync(x => x.RoleId, x => x.RoleName);
                case nameof(Department):
                    return await _db.Departments.AsNoTracking().ToDictionaryAsync(x => x.DepartmentId, x => x.Name);
                case nameof(Designation):
                    return await _db.Designations.AsNoTracking().ToDictionaryAsync(x => x.DesignationId, x => x.Title);
                case nameof(JobOpening):
                    return await _db.JobOpenings.AsNoTracking().ToDictionaryAsync(x => x.JobOpeningId, x => x.Title);
                case nameof(Candidate):
                    return await _db.Candidates.AsNoTracking().ToDictionaryAsync(x => x.CandidateId, x => x.FullName);
                case nameof(LeaveType):
                    return await _db.LeaveTypes.AsNoTracking().ToDictionaryAsync(x => x.LeaveTypeId, x => x.Name);
                case nameof(JobApplication):
                    return await _db.JobApplications.AsNoTracking()
                        .Select(a => new { a.ApplicationId, Text = a.Candidate.FullName + " - " + a.JobOpening.Title })
                        .ToDictionaryAsync(x => x.ApplicationId, x => x.Text);
                case nameof(OfferLetter):
                    return await _db.OfferLetters.AsNoTracking()
                        .Select(o => new { o.OfferId, Text = "Offer to " + o.Application.Candidate.FullName })
                        .ToDictionaryAsync(x => x.OfferId, x => x.Text);
                case nameof(Interview):
                    return (await _db.Interviews.AsNoTracking()
                            .Select(i => new { i.InterviewId, i.Application.Candidate.FullName, i.ScheduledAt }).ToListAsync())
                        .ToDictionary(x => x.InterviewId, x => $"{x.FullName} ({x.ScheduledAt:dd MMM yyyy HH:mm})");
                case nameof(Attendance):
                    return (await _db.Attendances.AsNoTracking()
                            .Select(a => new { a.AttendanceId, a.Employee.EmployeeCode, a.Date }).ToListAsync())
                        .ToDictionary(x => x.AttendanceId, x => $"{x.EmployeeCode} - {x.Date:dd MMM yyyy}");
                case nameof(Payroll):
                    return (await _db.Payrolls.AsNoTracking()
                            .Select(p => new { p.PayrollId, p.Employee.EmployeeCode, p.Month, p.Year }).ToListAsync())
                        .ToDictionary(x => x.PayrollId, x => $"{x.EmployeeCode} - {x.Month:D2}/{x.Year}");
                case nameof(LeaveRequest):
                    return (await _db.LeaveRequests.AsNoTracking()
                            .Select(l => new { l.LeaveRequestId, l.Employee.EmployeeCode, l.FromDate, l.TotalDays }).ToListAsync())
                        .ToDictionary(x => x.LeaveRequestId, x => $"{x.EmployeeCode} - {x.FromDate:dd MMM yyyy} ({x.TotalDays} d)");
                case nameof(PerformanceReview):
                    return await _db.PerformanceReviews.AsNoTracking()
                        .Select(r => new { r.ReviewId, Text = r.Employee.EmployeeCode + " - " + r.Period })
                        .ToDictionaryAsync(x => x.ReviewId, x => x.Text);
            }

            var entityType = _db.Model.FindEntityType(type)!;
            var key = entityType.FindPrimaryKey()!.Properties[0].Name;
            var display = type.GetCustomAttribute<DisplayColumnAttribute>()?.DisplayColumn;
            if (display != null)
            {
                var method = typeof(RecordRepository).GetMethod(nameof(SimpleLookupAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
                return await (Task<Dictionary<int, string>>)method.MakeGenericMethod(type).Invoke(this, new object[] { key, display })!;
            }

            // Link tables: show the names of the first two parents, e.g. "Team A / EMP00001 - Rahim".
            var rows = await LoadAllAsync(type);
            var fks = entityType.GetForeignKeys().Take(2).ToList();
            var parentLookups = new List<(string Prop, Dictionary<int, string> Texts)>();
            if (depth < 2)
            {
                foreach (var fk in fks)
                    parentLookups.Add((fk.Properties[0].Name, await BuildLookupAsync(fk.PrincipalEntityType.ClrType, depth + 1)));
            }

            var result = new Dictionary<int, string>();
            foreach (var row in rows)
            {
                var id = (int)type.GetProperty(key)!.GetValue(row)!;
                var parts = parentLookups
                    .Select(p => type.GetProperty(p.Prop)!.GetValue(row) is int v && p.Texts.TryGetValue(v, out var t) ? t : null)
                    .Where(t => t != null).ToList();
                result[id] = parts.Count > 0 ? string.Join(" / ", parts) : $"{Words(type.Name)} #{id}";
            }
            return result;
        }

        private async Task<Dictionary<int, string>> SimpleLookupAsync<T>(string key, string display) where T : class
        {
            var list = await _db.Set<T>().AsNoTracking()
                .Select(e => new { Id = EF.Property<int>(e, key), Text = EF.Property<string>(e, display) })
                .ToListAsync();
            return list.ToDictionary(x => x.Id, x => x.Text ?? $"#{x.Id}");
        }

        // ---------------- One record with parents and children ----------------

        public async Task<RecordDetails?> GetDetailsAsync(Type type, int id)
        {
            var entityType = _db.Model.FindEntityType(type);
            if (entityType == null) return null;

            var entity = await FindAsync(type, entityType, id);
            if (entity == null) return null;

            var lookup = await GetLookupAsync(type);
            var details = new RecordDetails
            {
                EntityName = type.Name,
                Id = id,
                Title = lookup.TryGetValue(id, out var t) ? t : $"{Words(type.Name)} #{id}"
            };

            var fkByProp = entityType.GetForeignKeys().ToDictionary(f => f.Properties[0].Name, f => f.PrincipalEntityType.ClrType);
            foreach (var p in entityType.GetProperties())
            {
                if (HiddenColumns.Contains(p.Name) || p.PropertyInfo == null) continue;
                var value = p.PropertyInfo.GetValue(entity);
                var field = new RecordField { Label = LabelOf(p.PropertyInfo) };
                if (fkByProp.TryGetValue(p.Name, out var parentType) && value is int parentId)
                {
                    var parents = await GetLookupAsync(parentType);
                    field.Value = parents.TryGetValue(parentId, out var pt) ? pt : $"#{parentId} (deleted)";
                    field.LinkEntity = parentType.Name;
                    field.LinkId = parentId;
                }
                else
                {
                    field.Value = FormatValue(value, p);
                }
                details.Fields.Add(field);
            }

            foreach (var fk in entityType.GetReferencingForeignKeys())
            {
                var childType = fk.DeclaringEntityType;
                var fkProp = fk.Properties[0].Name;
                var group = await LoadChildrenAsync(childType, fkProp, id);
                var sameParentCount = childType.GetForeignKeys().Count(f => f.PrincipalEntityType == entityType);
                group.Title = Words(Plural(childType.ClrType.Name)) +
                              (sameParentCount > 1 ? " (" + LabelOf(childType.ClrType.GetProperty(fkProp)!) + ")" : "");
                details.Children.Add(group);
            }
            details.Children = details.Children.OrderByDescending(c => c.Total).ThenBy(c => c.Title).ToList();
            return details;
        }

        private async Task<RecordChildGroup> LoadChildrenAsync(IEntityType childType, string fkProp, int parentId)
        {
            var clr = childType.ClrType;
            var key = childType.FindPrimaryKey()!.Properties[0].Name;
            var method = typeof(RecordRepository).GetMethod(nameof(LoadByForeignKeyAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
            var task = (Task<(List<object> Rows, int Total)>)method.MakeGenericMethod(clr).Invoke(this, new object[] { fkProp, parentId, key })!;
            var (rows, total) = await task;

            var fkByProp = childType.GetForeignKeys().ToDictionary(f => f.Properties[0].Name, f => f.PrincipalEntityType.ClrType);
            var columns = childType.GetProperties()
                .Where(p => p.PropertyInfo != null && !HiddenColumns.Contains(p.Name) && p.Name != fkProp && !p.IsPrimaryKey())
                .Where(p => p.ClrType != typeof(string) || (p.GetMaxLength() ?? int.MaxValue) <= 300)
                .Take(7).ToList();

            var group = new RecordChildGroup { EntityName = clr.Name, ForeignKey = fkProp, Total = total };
            group.Columns = columns.Select(c => LabelOf(c.PropertyInfo!)).ToList();
            var parentLookups = new Dictionary<string, Dictionary<int, string>>();
            foreach (var c in columns.Where(c => fkByProp.ContainsKey(c.Name)))
                parentLookups[c.Name] = await GetLookupAsync(fkByProp[c.Name]);

            foreach (var row in rows)
            {
                var r = new RecordRow { Id = (int)clr.GetProperty(key)!.GetValue(row)! };
                foreach (var c in columns)
                {
                    var v = c.PropertyInfo!.GetValue(row);
                    if (parentLookups.TryGetValue(c.Name, out var texts))
                        r.Cells.Add(v is int pid && texts.TryGetValue(pid, out var txt) ? txt : "-");
                    else
                        r.Cells.Add(FormatValue(v, c));
                }
                group.Rows.Add(r);
            }
            return group;
        }

        private async Task<(List<object> Rows, int Total)> LoadByForeignKeyAsync<T>(string fkProp, int id, string key) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = Expression.Convert(Expression.Property(parameter, fkProp), typeof(int?));
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(left, Expression.Constant(id, typeof(int?))), parameter);
            var query = _db.Set<T>().AsNoTracking().Where(predicate);
            var total = await query.CountAsync();
            var rows = await query.OrderByDescending(x => EF.Property<int>(x, key)).Take(50).ToListAsync();
            return (rows.Cast<object>().ToList(), total);
        }

        private async Task<object?> FindAsync(Type type, IEntityType entityType, int id)
        {
            var key = entityType.FindPrimaryKey()!.Properties[0].Name;
            var method = typeof(RecordRepository).GetMethod(nameof(FindOneAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
            return await (Task<object?>)method.MakeGenericMethod(type).Invoke(this, new object[] { key, id })!;
        }

        private async Task<object?> FindOneAsync<T>(string key, int id) where T : class =>
            await _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => EF.Property<int>(x, key) == id);

        private async Task<List<object>> LoadAllAsync(Type type)
        {
            var method = typeof(RecordRepository).GetMethod(nameof(LoadAllGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
            return await (Task<List<object>>)method.MakeGenericMethod(type).Invoke(this, Array.Empty<object>())!;
        }

        private async Task<List<object>> LoadAllGenericAsync<T>() where T : class =>
            (await _db.Set<T>().AsNoTracking().ToListAsync()).Cast<object>().ToList();

        // ---------------- All tables ----------------

        public async Task<List<TableSummary>> GetTablesAsync()
        {
            var list = new List<TableSummary>();
            var count = typeof(RecordRepository).GetMethod(nameof(CountAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;
            foreach (var et in _db.Model.GetEntityTypes().OrderBy(t => t.GetTableName()))
            {
                list.Add(new TableSummary
                {
                    EntityName = et.ClrType.Name,
                    TableName = et.GetTableName() ?? et.ClrType.Name,
                    Columns = et.GetProperties().Count(),
                    SoftDelete = typeof(ISoftDelete).IsAssignableFrom(et.ClrType),
                    Rows = await (Task<int>)count.MakeGenericMethod(et.ClrType).Invoke(this, Array.Empty<object>())!,
                    References = et.GetForeignKeys().Select(f => f.PrincipalEntityType.ClrType.Name).Distinct().ToList(),
                    ReferencedBy = et.GetReferencingForeignKeys().Select(f => f.DeclaringEntityType.ClrType.Name).Distinct().ToList()
                });
            }
            return list;
        }

        private async Task<int> CountAsync<T>() where T : class => await _db.Set<T>().CountAsync();

        // ---------------- Helpers ----------------

        private static string FormatValue(object? value, IProperty p) => value switch
        {
            null => "-",
            DateTime d when p.GetColumnType() == "date" => d.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
            DateTime d => d.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture),
            TimeSpan ts => ts.ToString(@"hh\:mm"),
            decimal m => m.ToString("N2", CultureInfo.InvariantCulture),
            bool b => b ? "Yes" : "No",
            _ => value.ToString() ?? "-"
        };

        public static string LabelOf(PropertyInfo prop)
        {
            var name = prop.GetCustomAttribute<DisplayAttribute>()?.GetName();
            if (!string.IsNullOrEmpty(name)) return name;
            var n = prop.Name.EndsWith("Id") && prop.Name.Length > 2 ? prop.Name[..^2] : prop.Name;
            return Words(n);
        }

        public static string Words(string name)
        {
            var s = Regex.Replace(name, "(?<=[a-z0-9])(?=[A-Z])", " ");
            return s.Length > 1 ? s[0] + s[1..].ToLowerInvariant() : s;
        }

        public static string Plural(string name) =>
            name.EndsWith("y") && !name.EndsWith("ay") ? name[..^1] + "ies"
            : name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("ch") ? name + "es"
            : name + "s";
    }
}
