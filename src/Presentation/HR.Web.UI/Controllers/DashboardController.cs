using HR.DataAccess.Reporting;
using HR.Entities;
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
        private readonly IJobApplicationFacade _applicationFacade;
        private readonly IReportFacade _reports;
        private readonly IRecordFacade _records;

        public DashboardController(IJobApplicationFacade applicationFacade, IReportFacade reports, IRecordFacade records)
        {
            _applicationFacade = applicationFacade;
            _reports = reports;
            _records = records;
        }

        // Filter bar like an HRM package: Branch, Department, Designation, Name, Employee code -> Apply.
        public async Task<IActionResult> Index([FromQuery] ReportFilter filter, bool apply = false, DateTime? date = null, int? dept = null)
        {
            if (User.IsInRole(RoleNames.Candidate))
                return RedirectToAction("MyApplications", "Careers");

            var vm = new HrmsDashboardViewModel();
            if (User.IsInRole(RoleNames.Employee))
                return View(vm); // employees only see the welcome cards

            vm.Stats = await _reports.GetDashboardAsync();
            vm.Date = (date ?? DateTime.Today).Date;
            vm.DepartmentId = dept;
            vm.Charts = await _reports.GetDashboardChartsAsync(vm.Date, dept);
            vm.TotalEmployees = vm.Stats.Employees;
            vm.OpenJobs = vm.Stats.OpenJobs;
            vm.TotalApplications = vm.Stats.Applications;
            vm.PendingLeaves = vm.Stats.PendingLeaves;

            var applications = await _applicationFacade.GetJobApplicationsAsync();
            vm.ApplicationsByStatus = ApplicationStatus.All.ToDictionary(s => s, s => applications.Count(a => a.Status == s));
            vm.RecentApplications = applications.Take(5).ToList();

            vm.Filter = filter;
            vm.Branches = await _records.GetLookupAsync(typeof(Branch));
            vm.Departments = await _records.GetLookupAsync(typeof(Department));
            vm.Designations = await _records.GetLookupAsync(typeof(Designation));
            if (apply)
            {
                vm.Searched = true;
                vm.Results = await _reports.SearchEmployeesAsync(filter);
            }
            return View(vm);
        }
    }
}
