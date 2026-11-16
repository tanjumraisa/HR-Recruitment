using HR.DataAccess.Reporting;

namespace HR.DataAccess.Interface
{
    public interface IRecordRepository
    {
        Type? FindType(string entityName);
        Task<Dictionary<int, string>> GetLookupAsync(Type type);
        Task<RecordDetails?> GetDetailsAsync(Type type, int id);
        Task<List<TableSummary>> GetTablesAsync();
    }

    public interface IReportRepository
    {
        Task<DashboardStats> GetDashboardAsync();
        Task<DashboardCharts> GetDashboardChartsAsync(DateTime date, int? departmentId);
        Task<List<EmployeeSearchRow>> SearchEmployeesAsync(ReportFilter filter);
        Task<ReportResult> EmployeeListAsync(ReportFilter filter);
        Task<ReportResult> HeadcountAsync();
        Task<ReportResult> LeaveSummaryAsync(int year);
        Task<ReportResult> AttendanceSummaryAsync(int month, int year);
        Task<ReportResult> SalarySheetAsync(int month, int year);
        Task<ReportResult> RecruitmentFunnelAsync();
        Task<ReportResult> TrainingReportAsync();
        Task<ReportResult> LoanOutstandingAsync();
        Task<ReportResult> ExpiryAlertAsync(int days);
        Task<ReportResult> OvertimeSummaryAsync(int month, int year);
        Task<ReportResult> NewJoinersAsync(int month, int year);
        Task<ReportResult> IncrementReportAsync(int year);
        Task<ReportResult> PunchMissingAsync(int month, int year);
        Task<ReportResult> FundBalanceAsync();
        Task<ReportResult> OutsourcedSalaryAsync(int month, int year);
        Task<ReportResult> TravelExpenseAsync(int month, int year);
    }

    public interface INotificationRepository : IRepository<HR.Entities.Notification>
    {
        Task<List<HR.Entities.Notification>> GetForUserAsync(int userId, int take);
        Task<int> CountUnreadAsync(int userId);
        Task MarkAllReadAsync(int userId);
    }

    public interface IAuditLogRepository : IRepository<HR.Entities.AuditLog>
    {
        Task<(List<HR.Entities.AuditLog> Items, int Total)> SearchAsync(string? q, int page, int pageSize);
    }
}
