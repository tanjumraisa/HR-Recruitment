using HR.DataAccess.Interface;
using HR.DataAccess.Reporting;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;

namespace HR.Facade.Implementation
{
    public class RecordFacade : IRecordFacade
    {
        private readonly IRecordRepository _repo;

        public RecordFacade(IRecordRepository repo)
        {
            _repo = repo;
        }

        public Type? FindType(string entityName) => _repo.FindType(entityName);

        public Task<Dictionary<int, string>> GetLookupAsync(Type type) => _repo.GetLookupAsync(type);

        public async Task<RecordDetails?> GetDetailsAsync(string entityName, int id)
        {
            var type = _repo.FindType(entityName);
            return type == null ? null : await _repo.GetDetailsAsync(type, id);
        }

        public Task<List<TableSummary>> GetTablesAsync() => _repo.GetTablesAsync();
    }

    public class ReportFacade : IReportFacade
    {
        private readonly IReportRepository _repo;

        public ReportFacade(IReportRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyList<ReportInfo> Reports { get; } = new List<ReportInfo>
        {
            new("employees", "Employee list", "Employees filtered by branch, department, designation and status", false, false, true),
            new("headcount", "Headcount by department and branch", "Number of staff and monthly salary cost", false, false, false),
            new("leave-summary", "Leave summary", "Leave taken and allowed per employee and leave type", false, true, false),
            new("attendance-summary", "Monthly attendance summary", "Present, late, absent, leave and overtime per employee", true, true, false),
            new("salary-sheet", "Salary sheet", "Payroll with bonus and loan deduction for a month", true, true, false),
            new("overtime", "Overtime register", "Overtime hours and amount of a month", true, true, false),
            new("recruitment-funnel", "Recruitment funnel", "Applications per stage for every job opening", false, false, false),
            new("training", "Training report", "Enrolment, completion and scores per training", false, false, false),
            new("loans", "Loan and advance outstanding", "Amount paid and outstanding per loan", false, false, false),
            new("expiry", "Contract and document expiry", "Contracts and documents expiring in the next 60 days", false, false, false),
            new("new-joiners", "Employee new join list", "Employees who joined in a month", true, true, false),
            new("increment", "Increment and promotion report", "Promotions and salary increments of a year", false, true, false),
            new("punch-missing", "In-punch and out-punch missing", "Attendance days with a missing check-in or check-out", true, true, false),
            new("fund-balance", "Fund balance", "Contributions, withdrawals and balance per fund member", false, false, false),
            new("outsourced-salary", "Outsourced salary sheet", "Wages of outsourced workers for a month", true, true, false),
            new("travel-expense", "TA/DA expense report", "Travel expense claims of a month", true, true, false)
        };

        public Task<DashboardStats> GetDashboardAsync() => _repo.GetDashboardAsync();

        public Task<DashboardCharts> GetDashboardChartsAsync(DateTime date, int? departmentId) => _repo.GetDashboardChartsAsync(date, departmentId);

        public Task<List<EmployeeSearchRow>> SearchEmployeesAsync(ReportFilter filter) => _repo.SearchEmployeesAsync(filter);

        public async Task<ReportResult?> RunAsync(string key, ReportFilter f)
        {
            var month = f.Month is >= 1 and <= 12 ? f.Month.Value : DateTime.Today.Month;
            var year = f.Year is >= 2000 and <= 2100 ? f.Year.Value : DateTime.Today.Year;
            return key switch
            {
                "employees" => await _repo.EmployeeListAsync(f),
                "headcount" => await _repo.HeadcountAsync(),
                "leave-summary" => await _repo.LeaveSummaryAsync(year),
                "attendance-summary" => await _repo.AttendanceSummaryAsync(month, year),
                "salary-sheet" => await _repo.SalarySheetAsync(month, year),
                "overtime" => await _repo.OvertimeSummaryAsync(month, year),
                "recruitment-funnel" => await _repo.RecruitmentFunnelAsync(),
                "training" => await _repo.TrainingReportAsync(),
                "loans" => await _repo.LoanOutstandingAsync(),
                "expiry" => await _repo.ExpiryAlertAsync(60),
                "new-joiners" => await _repo.NewJoinersAsync(month, year),
                "increment" => await _repo.IncrementReportAsync(year),
                "punch-missing" => await _repo.PunchMissingAsync(month, year),
                "fund-balance" => await _repo.FundBalanceAsync(),
                "outsourced-salary" => await _repo.OutsourcedSalaryAsync(month, year),
                "travel-expense" => await _repo.TravelExpenseAsync(month, year),
                _ => null
            };
        }
    }

    public class NotificationFacade : INotificationFacade
    {
        private readonly INotificationRepository _repo;
        private readonly IEmployeeRepository _employees;

        public NotificationFacade(INotificationRepository repo, IEmployeeRepository employees)
        {
            _repo = repo;
            _employees = employees;
        }

        public Task<List<Notification>> GetMineAsync(int userId, int take = 50) => _repo.GetForUserAsync(userId, take);

        public Task<int> CountUnreadAsync(int userId) => _repo.CountUnreadAsync(userId);

        public async Task<Notification?> OpenAsync(int id, int userId)
        {
            var n = await _repo.GetByIdAsync(id);
            if (n == null || n.UserId != userId) return null;
            if (!n.IsRead)
            {
                n.IsRead = true;
                await _repo.UpdateAsync(n);
            }
            return n;
        }

        public Task MarkAllReadAsync(int userId) => _repo.MarkAllReadAsync(userId);

        public async Task NotifyUserAsync(int userId, string title, string message, string? link = null)
        {
            if (userId <= 0) return;
            await _repo.AddAsync(new Notification { UserId = userId, Title = title, Message = message, Link = link, CreatedAt = DateTime.Now });
        }

        public async Task NotifyEmployeeAsync(int employeeId, string title, string message, string? link = null)
        {
            var employee = await _employees.GetByIdAsync(employeeId);
            if (employee != null) await NotifyUserAsync(employee.UserId, title, message, link);
        }
    }

    public class AuditLogFacade : IAuditLogFacade
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogFacade(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public Task<(List<AuditLog> Items, int Total)> SearchAsync(string? q, int page, int pageSize) =>
            _repo.SearchAsync(q?.Trim(), Math.Max(1, page), pageSize);
    }
}
