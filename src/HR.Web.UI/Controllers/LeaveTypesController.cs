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
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class LeaveTypesController : AppController
    {
        private readonly ILeaveTypeFacade _facade;

        public LeaveTypesController(ILeaveTypeFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetLeaveTypesAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new LeaveType());
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddLeaveTypeAsync(model);
                    TempData["Success"] = "Leave type created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _facade.GetLeaveTypeByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeaveType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateLeaveTypeAsync(model)) return NotFound();
                    TempData["Success"] = "Leave type updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (BusinessException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _facade.GetLeaveTypeByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete leave type", Message = e.Name });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteLeaveTypeAsync(id)) throw new BusinessException("Record not found.");
            }, "Leave type deleted.");


        private async Task LoadLookupsAsync()
        {
            await Task.CompletedTask;
        }
    }
}
