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
    [Authorize(Roles = RoleNames.Staff)]
    public class CandidatesController : AppController
    {
        private readonly ICandidateFacade _facade;

        public CandidatesController(ICandidateFacade facade)
        {
            _facade = facade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetCandidatesAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new Candidate());
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddCandidateAsync(model);
                    TempData["Success"] = "Candidate created.";
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

        [Authorize(Roles = RoleNames.Staff)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _facade.GetCandidateByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Candidate model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateCandidateAsync(model)) return NotFound();
                    TempData["Success"] = "Candidate updated.";
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
            var e = await _facade.GetCandidateByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete candidate", Message = e.FullName });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteCandidateAsync(id)) throw new BusinessException("Record not found.");
            }, "Candidate deleted.");


        private async Task LoadLookupsAsync()
        {
            await Task.CompletedTask;
        }
    }
}
