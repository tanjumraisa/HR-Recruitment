using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    public class CareersController : AppController
    {
        private readonly IJobOpeningFacade _jobFacade;
        private readonly IJobApplicationFacade _applicationFacade;
        private readonly ICandidateFacade _candidateFacade;

        public CareersController(IJobOpeningFacade jobFacade, IJobApplicationFacade applicationFacade, ICandidateFacade candidateFacade)
        {
            _jobFacade = jobFacade;
            _applicationFacade = applicationFacade;
            _candidateFacade = candidateFacade;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? q)
        {
            ViewBag.Query = q;
            return View(await _jobFacade.GetOpenJobsAsync(q));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobFacade.GetJobOpeningByIdAsync(id);
            if (job == null || job.Status != JobStatus.Open) return NotFound();
            return View(job);
        }

        [Authorize(Roles = RoleNames.Candidate)]
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _jobFacade.GetJobOpeningByIdAsync(id);
            if (job == null || job.Status != JobStatus.Open) return NotFound();
            return View(job);
        }

        [Authorize(Roles = RoleNames.Candidate)]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id, string? coverLetter)
        {
            var candidate = await _candidateFacade.GetCandidateByUserIdAsync(User.GetUserId());
            if (candidate == null)
            {
                TempData["Error"] = "Candidate profile not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _applicationFacade.ApplyAsync(id, candidate.CandidateId, coverLetter);
                TempData["Success"] = "Application submitted successfully.";
                return RedirectToAction(nameof(MyApplications));
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [Authorize(Roles = RoleNames.Candidate)]
        public async Task<IActionResult> MyApplications()
        {
            var candidate = await _candidateFacade.GetCandidateByUserIdAsync(User.GetUserId());
            var list = candidate == null ? new List<JobApplication>() : await _applicationFacade.GetByCandidateAsync(candidate.CandidateId);
            return View(list);
        }
    }
}
