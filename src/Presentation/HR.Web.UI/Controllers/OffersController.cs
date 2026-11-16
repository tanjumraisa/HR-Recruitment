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
    public class OffersController : AppController
    {
        private readonly IOfferLetterFacade _facade;
        private readonly IJobApplicationFacade _jobApplicationFacade;

        public OffersController(IOfferLetterFacade facade, IJobApplicationFacade jobApplicationFacade)
        {
            _facade = facade;
            _jobApplicationFacade = jobApplicationFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetOfferLettersAsync();
            return View(items);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View("Form", new OfferLetter { JoiningDate = DateTime.Today.AddDays(30) });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfferLetter model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _facade.AddOfferLetterAsync(model);
                    TempData["Success"] = "Offer created.";
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
            var model = await _facade.GetOfferLetterByIdAsync(id);
            if (model == null) return NotFound();
            await LoadLookupsAsync();
            return View("Form", model);
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OfferLetter model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _facade.UpdateOfferLetterAsync(model)) return NotFound();
                    TempData["Success"] = "Offer updated.";
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
            var e = await _facade.GetOfferLetterByIdAsync(id);
            if (e == null) return NotFound();
            return View("~/Views/Shared/DeleteConfirm.cshtml",
                new DeleteViewModel { Id = id, Title = "Delete offer", Message = e.Application.Candidate.FullName });
        }

        [Authorize(Roles = RoleNames.HrAdmin)]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteConfirmed(int id) =>
            TryAsync(async () =>
            {
                if (!await _facade.DeleteOfferLetterAsync(id)) throw new BusinessException("Record not found.");
            }, "Offer deleted.");

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = RoleNames.HrAdmin)]
        public Task<IActionResult> Respond(int id, bool accept) =>
            TryAsync(() => _facade.RespondAsync(id, accept), accept ? "Offer accepted: candidate hired." : "Offer declined.");

        private async Task LoadLookupsAsync()
        {
            ViewBag.Applications = (await _jobApplicationFacade.GetJobApplicationsAsync()).Select(x => new SelectListItem(x.Candidate.FullName + " - " + x.JobOpening.Title, x.ApplicationId.ToString())).ToList();
            await Task.CompletedTask;
        }
    }
}
