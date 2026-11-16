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
    public class JobsController : AppController
    {
        private readonly IJobOpeningFacade _facade;
        private readonly IDepartmentFacade _departmentFacade;
        private readonly IDesignationFacade _designationFacade;

        public JobsController(IJobOpeningFacade facade, IDepartmentFacade departmentFacade, IDesignationFacade designationFacade)
        {
            _facade = facade;
            _departmentFacade = departmentFacade;
            _designationFacade = designationFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetJobOpeningsAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new JobOpening { Deadline = DateTime.Today.AddDays(30) });
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobOpening model)
        {
            model.PostedById = User.GetUserId();
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddJobOpeningAsync(model);
                    TempData["Success"] = "Job opening created.";
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
            var model = await _facade.GetJobOpeningByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobOpening model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateJobOpeningAsync(model)) return NotFound();
                    TempData["Success"] = "Job opening updated.";
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
            var e = await _facade.GetJobOpeningByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete job opening", Message = e.Title });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteJobOpeningAsync(id)) throw new BusinessException("Record not found.");
            }, "Job opening deleted.");

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> SetStatus(int id, string status) =>
            TryAsync(() => _facade.SetStatusAsync(id, status), $"Job status changed to {status}.");

        private async Task LoadLookupsAsync()
        {
            ViewBag.Departments = (await _departmentFacade.GetDepartmentsAsync()).Select(x => new SelectListItem(x.Name, x.DepartmentId.ToString())).ToList();
            ViewBag.Designations = (await _designationFacade.GetDesignationsAsync()).Select(x => new SelectListItem(x.Title, x.DesignationId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
