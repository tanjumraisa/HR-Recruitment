using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    [Authorize]
    public class DashboardController : AppController
    {
        private readonly IEmployeeFacade _employeeFacade;
        private readonly IJobOpeningFacade _jobFacade;
        private readonly IJobApplicationFacade _applicationFacade;
        private readonly ILeaveRequestFacade _leaveFacade;

        public DashboardController(IEmployeeFacade employeeFacade, IJobOpeningFacade jobFacade,
            IJobApplicationFacade applicationFacade, ILeaveRequestFacade leaveFacade)
        {
            _employeeFacade = employeeFacade;
            _jobFacade = jobFacade;
            _applicationFacade = applicationFacade;
            _leaveFacade = leaveFacade;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(RoleNames.Candidate))
                return RedirectToAction("MyApplications", "Careers");

            var vm = new DashboardViewModel();
            if (User.IsInRole(RoleNames.Employee))
                return View(vm); // employees only see the welcome cards

            var employees = await _employeeFacade.GetEmployeesAsync();
            var jobs = await _jobFacade.GetJobOpeningsAsync();
            var applications = await _applicationFacade.GetJobApplicationsAsync();

            vm.TotalEmployees = employees.Count(e => e.Status == EmployeeStatus.Active);
            vm.OpenJobs = jobs.Count(j => j.Status == JobStatus.Open);
            vm.TotalApplications = applications.Count;
            vm.ApplicationsByStatus = ApplicationStatus.All.ToDictionary(s => s, s => applications.Count(a => a.Status == s));
            vm.RecentApplications = applications.Take(5).ToList();

            if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.HRManager))
                vm.PendingLeaves = (await _leaveFacade.GetLeaveRequestsAsync()).Count(l => l.Status == LeaveStatus.Pending);

            return View(vm);
        }
    }
}
