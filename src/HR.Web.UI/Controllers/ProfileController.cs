using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    [Authorize(Roles = RoleNames.Candidate)]
    public class ProfileController : AppController
    {
        private readonly ICandidateFacade _candidateFacade;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ICandidateFacade candidateFacade, IWebHostEnvironment env)
        {
            _candidateFacade = candidateFacade;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var candidate = await _candidateFacade.GetCandidateByUserIdAsync(User.GetUserId());
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(IFormFile? resume, string? phone, string? skills, int experienceYears)
        {
            var candidate = await _candidateFacade.GetCandidateByUserIdAsync(User.GetUserId());
            if (candidate == null) return NotFound();

            try
            {
                candidate.Phone = phone;
                candidate.Skills = skills;
                candidate.ExperienceYears = experienceYears;
                await _candidateFacade.UpdateCandidateAsync(candidate);

                if (resume != null && resume.Length > 0)
                {
                    var (stored, original) = await ResumeStorage.SaveAsync(_env, resume);
                    var oldFile = candidate.ResumePath;
                    await _candidateFacade.SetResumeAsync(candidate.CandidateId, stored, original);
                    ResumeStorage.Delete(_env, oldFile);
                }
                TempData["Success"] = "Profile saved.";
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
