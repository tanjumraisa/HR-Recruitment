using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR.Web.UI.Controllers
{
    [Authorize(Roles = RoleNames.AdminOnly)]
    public class UsersController : AppController
    {
        private readonly IUserFacade _facade;
        private readonly IRoleFacade _roleFacade;

        public UsersController(IUserFacade facade, IRoleFacade roleFacade)
        {
            _facade = facade;
            _roleFacade = roleFacade;
        }

        public async Task<IActionResult> Index() => View(await _facade.GetUsersAsync());

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View(new UserCreateViewModel { RoleId = RoleIds.Recruiter });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddUserAsync(new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.Phone,
                        PasswordHash = model.Password, // hashed inside the facade
                        RoleId = model.RoleId
                    });
                    TempData["Success"] = "User created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadRolesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _facade.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            await LoadRolesAsync();
            return View(new UserEditViewModel { UserId = user.UserId, FullName = user.FullName, Email = user.Email, RoleId = user.RoleId, IsActive = user.IsActive });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var user = await _facade.GetUserByIdAsync(model.UserId);
            if (user == null) return NotFound();

            if (user.UserId == User.GetUserId() && (!model.IsActive || model.RoleId != RoleIds.Admin))
            {
                ModelState.AddModelError(string.Empty, "You cannot deactivate or demote your own account.");
            }
            else
            {
                try
                {
                    user.RoleId = model.RoleId;
                    user.IsActive = model.IsActive;
                    await _facade.UpdateUserAsync(user);
                    TempData["Success"] = "User updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadRolesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _facade.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml", new DeleteViewModel { Id = id, Title = "Delete user", Message = user.FullName });
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == User.GetUserId())
            {
                TempData["Error"] = "You cannot delete your own account.";
                return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
            }
            return TryAsync(async () =>
            {
                if (!await _facade.DeleteUserAsync(id)) throw new BusinessException("User not found.");
            }, "User deleted.");
        }

        private async Task LoadRolesAsync()
        {
            ViewBag.Roles = (await _roleFacade.GetRolesAsync())
                .Select(r => new SelectListItem(r.RoleName, r.RoleId.ToString())).ToList();
        }
    }
}
