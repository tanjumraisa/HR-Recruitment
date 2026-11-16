using HR.Integration.Reporting;
using HR.DataAccess.Reporting;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    [Authorize(Roles = RoleNames.Staff)]
    public class ReportsController : AppController
    {
        private readonly IReportFacade _reports;
        private readonly IRecordFacade _records;
        private readonly ICsvWriter _csv;

        public ReportsController(IReportFacade reports, IRecordFacade records, ICsvWriter csv)
        {
            _csv = csv;
            _reports = reports;
            _records = records;
        }

        public IActionResult Index() => View(_reports.Reports);

        public async Task<IActionResult> Show(string id, [FromQuery] ReportFilter filter, string? export)
        {
            var info = _reports.Reports.FirstOrDefault(r => r.Key == id);
            if (info == null) return NotFound();
            filter.Month ??= DateTime.Today.Month;
            filter.Year ??= DateTime.Today.Year;
            var result = await _reports.RunAsync(id, filter);
            if (result == null) return NotFound();

            if (export == "csv")
            {
                var rows = result.Footer != null ? result.Rows.Append(result.Footer) : result.Rows;
                var bytes = _csv.Write(result.Columns, rows);
                return File(bytes, "text/csv", id + ".csv");
            }

            return View(new ReportViewModel
            {
                Info = info,
                Result = result,
                Filter = filter,
                Branches = await _records.GetLookupAsync(typeof(Branch)),
                Departments = await _records.GetLookupAsync(typeof(Department)),
                Designations = await _records.GetLookupAsync(typeof(Designation))
            });
        }
    }
}
