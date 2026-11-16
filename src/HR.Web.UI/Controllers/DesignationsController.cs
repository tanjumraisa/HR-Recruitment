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
    public class DesignationsController : AppController
    {
        private readonly IDesignationFacade _facade;

        public DesignationsController(IDesignationFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetDesignationsAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new Designation());
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddDesignationAsync(model);
                    TempData["Success"] = "Designation created.";
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
            var model = await _facade.GetDesignationByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Designation model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateDesignationAsync(model)) return NotFound();
                    TempData["Success"] = "Designation updated.";
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
            var e = await _facade.GetDesignationByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete designation", Message = e.Title });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteDesignationAsync(id)) throw new BusinessException("Record not found.");
            }, "Designation deleted.");


        private async Task LoadLookupsAsync()
        {
            await Task.CompletedTask;
        }
    }
}
