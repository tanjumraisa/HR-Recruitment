using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    // Software Settings -> User permissions. Admin makes permission groups (tick boxes)
    // and gives groups or single permissions to users.
    [Authorize(Roles = RoleNames.AdminOnly)]
    public class PermissionsController : AppController
    {
        private readonly IPermissionFacade _permissions;
        private readonly IUserFacade _users;

        public PermissionsController(IPermissionFacade permissions, IUserFacade users)
        {
            _permissions = permissions;
            _users = users;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Groups = await _permissions.GetGroupsAsync();
            var users = (await _users.GetUsersAsync()).Where(u => u.RoleId != RoleIds.Admin && u.RoleId != RoleIds.Candidate).OrderBy(u => u.FullName).ToList();
            var assigned = new Dictionary<int, (HashSet<int> G, HashSet<int> P)>();
            foreach (var u in users) assigned[u.UserId] = await _permissions.GetUserAssignmentAsync(u.UserId);
            ViewBag.Assigned = assigned;
            return View(users);
        }

        public async Task<IActionResult> Group(int id = 0)
        {
            ViewBag.All = await _permissions.GetAllPermissionsAsync();
            if (id == 0) return View(new GroupForm());
            var (g, ids) = await _permissions.GetGroupAsync(id);
            return View(new GroupForm { Id = g.PermissionGroupId, Name = g.Name, Tag = g.Tag, PermissionIds = ids.ToList() });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Group(GroupForm form)
        {
            try
            {
                await _permissions.SaveGroupAsync(form.Id, form.Name, form.Tag, form.PermissionIds);
                PermissionFilter.ClearCache();
                TempData["Success"] = $"Permission group '{form.Name}' saved.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.All = await _permissions.GetAllPermissionsAsync();
                return View(form);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteGroup(int id) =>
            TryAsync(async () => { await _permissions.DeleteGroupAsync(id); PermissionFilter.ClearCache(); }, "Permission group deleted.");

        public async Task<IActionResult> Assign(int id)
        {
            var user = await _users.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            var (groups, perms) = await _permissions.GetUserAssignmentAsync(id);
            ViewBag.All = await _permissions.GetAllPermissionsAsync();
            ViewBag.Groups = await _permissions.GetGroupsAsync();
            return View(new UserForm { UserId = id, UserName = user.FullName, Email = user.Email, RoleId = user.RoleId, GroupIds = groups.ToList(), PermissionIds = perms.ToList() });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(UserForm form)
        {
            try
            {
                await _permissions.SaveUserAssignmentAsync(form.UserId, form.GroupIds, form.PermissionIds);
                PermissionFilter.ClearCache();
                TempData["Success"] = "User permissions saved.";
            }
            catch (BusinessException ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(Index));
        }

        public class GroupForm
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string? Tag { get; set; }
            public List<int> PermissionIds { get; set; } = new();
        }

        public class UserForm
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
            public int RoleId { get; set; }
            public List<int> GroupIds { get; set; } = new();
            public List<int> PermissionIds { get; set; } = new();
        }
    }
}
