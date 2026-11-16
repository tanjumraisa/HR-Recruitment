using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    [Authorize]
    public class ResumesController : Controller
    {
        private readonly ICandidateFacade _candidateFacade;
        private readonly IWebHostEnvironment _env;

        public ResumesController(ICandidateFacade candidateFacade, IWebHostEnvironment env)
        {
            _candidateFacade = candidateFacade;
            _env = env;
        }

        // Only staff and the candidate who owns the CV can download it.
        public async Task<IActionResult> Download(int id)
        {
            var candidate = await _candidateFacade.GetCandidateByIdAsync(id);
            if (candidate == null || string.IsNullOrEmpty(candidate.ResumePath)) return NotFound();

            var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.HRManager) || User.IsInRole(RoleNames.Recruiter);
            var isOwner = candidate.UserId.HasValue && candidate.UserId.Value == User.GetUserId();
            if (!isStaff && !isOwner) return Forbid();

            var path = ResumeStorage.FullPath(_env, candidate.ResumePath);
            if (!System.IO.File.Exists(path)) return NotFound();

            var ext = Path.GetExtension(path).ToLowerInvariant();
            var type = ext == ".pdf" ? "application/pdf"
                : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            return PhysicalFile(path, type, candidate.ResumeFileName ?? ("resume" + ext));
        }
    }
}
