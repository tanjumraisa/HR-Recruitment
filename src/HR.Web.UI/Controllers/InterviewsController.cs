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
    public class InterviewsController : AppController
    {
        private readonly IInterviewFacade _facade;
        private readonly IJobApplicationFacade _jobApplicationFacade;
        private readonly IEmployeeFacade _employeeFacade;

        public InterviewsController(IInterviewFacade facade, IJobApplicationFacade jobApplicationFacade, IEmployeeFacade employeeFacade)
        {
            _facade = facade;
            _jobApplicationFacade = jobApplicationFacade;
            _employeeFacade = employeeFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetInterviewsAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new Interview { ScheduledAt = DateTime.Today.AddDays(1).AddHours(10) });
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Interview model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddInterviewAsync(model);
                    TempData["Success"] = "Interview created.";
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
            var model = await _facade.GetInterviewByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Interview model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateInterviewAsync(model)) return NotFound();
                    TempData["Success"] = "Interview updated.";
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
            var e = await _facade.GetInterviewByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete interview", Message = e.Application.Candidate.FullName + " @ " + e.ScheduledAt.ToString("g") });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteInterviewAsync(id)) throw new BusinessException("Record not found.");
            }, "Interview deleted.");


        private async Task LoadLookupsAsync()
        {
            ViewBag.Applications = (await _jobApplicationFacade.GetJobApplicationsAsync()).Select(x => new SelectListItem(x.Candidate.FullName + " - " + x.JobOpening.Title, x.ApplicationId.ToString())).ToList();
            ViewBag.Interviewers = (await _employeeFacade.GetEmployeesAsync()).Select(x => new SelectListItem(x.EmployeeCode + " - " + x.User.FullName, x.EmployeeId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
