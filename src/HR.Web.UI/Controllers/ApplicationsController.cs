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
    public class ApplicationsController : AppController
    {
        private readonly IJobApplicationFacade _facade;
        private readonly IJobOpeningFacade _jobOpeningFacade;
        private readonly ICandidateFacade _candidateFacade;
        private readonly IInterviewFacade _interviewFacade;
        private readonly IOfferLetterFacade _offerLetterFacade;

        public ApplicationsController(IJobApplicationFacade facade, IJobOpeningFacade jobOpeningFacade, ICandidateFacade candidateFacade, IInterviewFacade interviewFacade, IOfferLetterFacade offerLetterFacade)
        {
            _facade = facade;
            _jobOpeningFacade = jobOpeningFacade;
            _candidateFacade = candidateFacade;
            _interviewFacade = interviewFacade;
            _offerLetterFacade = offerLetterFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetJobApplicationsAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new JobApplication());
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplication model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddJobApplicationAsync(model);
                    TempData["Success"] = "Application created.";
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
            var model = await _facade.GetJobApplicationByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.Staff)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobApplication model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateJobApplicationAsync(model)) return NotFound();
                    TempData["Success"] = "Application updated.";
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
            var e = await _facade.GetJobApplicationByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete application", Message = e.Candidate.FullName + " - " + e.JobOpening.Title });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteJobApplicationAsync(id)) throw new BusinessException("Record not found.");
            }, "Application deleted.");

        public async Task<IActionResult> Details(int id)
        {
            var app = await _facade.GetJobApplicationByIdAsync(id);
            if (app == null) return NotFound();

            ViewBag.Interviews = (await _interviewFacade.GetInterviewsAsync()).Where(i => i.ApplicationId == id).ToList();
            ViewBag.Offer = (await _offerLetterFacade.GetOfferLettersAsync()).FirstOrDefault(o => o.ApplicationId == id);
            return View(app);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            try
            {
                await _facade.ChangeStatusAsync(id, status);
                TempData["Success"] = $"Application marked as {status}.";
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadLookupsAsync()
        {
            ViewBag.Jobs = (await _jobOpeningFacade.GetJobOpeningsAsync()).Select(x => new SelectListItem(x.Title + " (" + x.Department.Name + ")", x.JobOpeningId.ToString())).ToList();
            ViewBag.Candidates = (await _candidateFacade.GetCandidatesAsync()).Select(x => new SelectListItem(x.FullName + " <" + x.Email + ">", x.CandidateId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
