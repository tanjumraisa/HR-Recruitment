using HR.DataAccess.Reporting;
using HR.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Endpoints
{
    // Read-only endpoints: dashboard, reports, connected records and the modules
    // that existed before the HRMS extension (returned as small DTOs, never with password hashes).
    public static class QueryEndpoints
    {
        public static RouteGroupBuilder MapHrQueries(this RouteGroupBuilder api)
        {
            var q = api.MapGroup("").WithTags("Queries");

            q.MapGet("/dashboard", async ([FromServices] IReportFacade r) => Results.Ok(await r.GetDashboardAsync()));

            q.MapGet("/employees/search", async ([AsParameters] ReportFilter filter, [FromServices] IReportFacade r) =>
                Results.Ok(await r.SearchEmployeesAsync(filter)));

            q.MapGet("/reports", ([FromServices] IReportFacade r) => Results.Ok(r.Reports));
            q.MapGet("/reports/{key}", async (string key, [AsParameters] ReportFilter filter, [FromServices] IReportFacade r) =>
                await r.RunAsync(key, filter) is { } result ? Results.Ok(result) : Results.NotFound());

            q.MapGet("/tables", async ([FromServices] IRecordFacade r) => Results.Ok(await r.GetTablesAsync()));
            q.MapGet("/records/{entity}/{id:int}", async (string entity, int id, [FromServices] IRecordFacade r) =>
                await r.GetDetailsAsync(entity, id) is { } d ? Results.Ok(d) : Results.NotFound());
            q.MapGet("/lookups/{entity}", async (string entity, [FromServices] IRecordFacade r) =>
                r.FindType(entity) is { } t ? Results.Ok(await r.GetLookupAsync(t)) : Results.NotFound());

            q.MapGet("/employees", async ([FromServices] IEmployeeFacade f) => Results.Ok((await f.GetEmployeesAsync()).Select(e => new
            {
                e.EmployeeId, e.EmployeeCode, Name = e.User?.FullName, Email = e.User?.Email, e.DepartmentId, Department = e.Department?.Name,
                e.DesignationId, Designation = e.Designation?.Title, e.BranchId, e.ManagerId, e.JoinDate, e.Salary, e.Status
            })));
            q.MapGet("/departments", async ([FromServices] IDepartmentFacade f) =>
                Results.Ok((await f.GetDepartmentsAsync()).Select(d => new { d.DepartmentId, d.Name, d.Description, d.IsActive })));
            q.MapGet("/designations", async ([FromServices] IDesignationFacade f) =>
                Results.Ok((await f.GetDesignationsAsync()).Select(d => new { d.DesignationId, d.Title, d.BaseSalary })));
            q.MapGet("/jobs", async ([FromServices] IJobOpeningFacade f) => Results.Ok((await f.GetJobOpeningsAsync()).Select(j => new
            {
                j.JobOpeningId, j.Title, j.DepartmentId, j.DesignationId, j.Vacancies, j.EmploymentType, j.SalaryMin, j.SalaryMax, j.Deadline, j.Status
            })));
            q.MapGet("/candidates", async ([FromServices] ICandidateFacade f) => Results.Ok((await f.GetCandidatesAsync()).Select(c => new
            {
                c.CandidateId, c.FullName, c.Email, c.Phone, c.Skills, c.ExperienceYears, HasCv = c.ResumeFileName != null, c.CreatedAt
            })));
            q.MapGet("/applications", async ([FromServices] IJobApplicationFacade f) => Results.Ok((await f.GetJobApplicationsAsync()).Select(a => new
            {
                a.ApplicationId, a.JobOpeningId, Job = a.JobOpening?.Title, a.CandidateId, Candidate = a.Candidate?.FullName, a.Status, a.AppliedAt
            })));
            q.MapGet("/interviews", async ([FromServices] IInterviewFacade f) => Results.Ok((await f.GetInterviewsAsync()).Select(i => new
            {
                i.InterviewId, i.ApplicationId, i.InterviewerId, i.ScheduledAt, i.Mode, i.Location, i.Status, i.Score
            })));
            q.MapGet("/offers", async ([FromServices] IOfferLetterFacade f) => Results.Ok((await f.GetOfferLettersAsync()).Select(o => new
            {
                o.OfferId, o.ApplicationId, o.OfferedSalary, o.JoiningDate, o.Status, o.IssuedAt
            })));
            q.MapGet("/leave-types", async ([FromServices] ILeaveTypeFacade f) =>
                Results.Ok((await f.GetLeaveTypesAsync()).Select(t => new { t.LeaveTypeId, t.Name, t.DaysPerYear })));
            q.MapGet("/leave-requests", async ([FromServices] ILeaveRequestFacade f) => Results.Ok((await f.GetLeaveRequestsAsync()).Select(l => new
            {
                l.LeaveRequestId, l.EmployeeId, l.LeaveTypeId, l.FromDate, l.ToDate, l.TotalDays, l.Status, l.Reason
            })));
            q.MapGet("/attendance", async ([FromServices] IAttendanceFacade f) => Results.Ok((await f.GetAttendancesAsync()).Select(a => new
            {
                a.AttendanceId, a.EmployeeId, a.Date, a.CheckIn, a.CheckOut, a.Status
            })));
            q.MapGet("/payrolls", async ([FromServices] IPayrollFacade f) => Results.Ok((await f.GetPayrollsAsync()).Select(p => new
            {
                p.PayrollId, p.EmployeeId, p.Month, p.Year, p.BasicSalary, p.Allowance, p.Deduction, p.NetSalary, p.Status
            })));
            q.MapGet("/performance-reviews", async ([FromServices] IPerformanceReviewFacade f) => Results.Ok((await f.GetPerformanceReviewsAsync()).Select(r => new
            {
                r.ReviewId, r.EmployeeId, r.ReviewerId, r.Period, r.Rating, r.Comments, r.ReviewedAt
            })));
            return api;
        }
    }

    public static class EndpointCatalog
    {
        public static object Describe() => new
        {
            name = "HR and Recruitment API",
            auth = "Send header X-Api-Key with every /api request.",
            readOnly = new[] { "/api/dashboard", "/api/employees/search", "/api/reports", "/api/reports/{key}", "/api/tables",
                "/api/records/{entity}/{id}", "/api/lookups/{entity}", "/api/employees", "/api/departments", "/api/designations", "/api/jobs",
                "/api/candidates", "/api/applications", "/api/interviews", "/api/offers", "/api/leave-types", "/api/leave-requests",
                "/api/attendance", "/api/payrolls", "/api/performance-reviews" },
            crud = ModuleEndpoints.Modules.Select(m => new { route = "/api/" + m.Route, entity = m.Entity })
        };
    }
}
