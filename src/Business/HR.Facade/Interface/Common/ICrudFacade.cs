using HR.DataAccess.Reporting;
using HR.Entities;

namespace HR.Facade.Interface
{
    // Standard operations of every HRMS module: insert, update, soft delete, get all active, get by id.
    public interface ICrudFacade<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(int id);
    }

    // Generic read access for the connected-records page, dropdowns and the table overview.
    public interface IRecordFacade
    {
        Type? FindType(string entityName);
        Task<Dictionary<int, string>> GetLookupAsync(Type type);
        Task<RecordDetails?> GetDetailsAsync(string entityName, int id);
        Task<List<TableSummary>> GetTablesAsync();
    }

    public interface IReportFacade
    {
        Task<DashboardStats> GetDashboardAsync();
        Task<DashboardCharts> GetDashboardChartsAsync(DateTime date, int? departmentId);
        Task<List<EmployeeSearchRow>> SearchEmployeesAsync(ReportFilter filter);
        IReadOnlyList<ReportInfo> Reports { get; }
        Task<ReportResult?> RunAsync(string key, ReportFilter filter);
    }

    public record ReportInfo(string Key, string Title, string Description, bool NeedsMonth, bool NeedsYear, bool NeedsFilters);

    public interface INotificationFacade
    {
        Task<List<Notification>> GetMineAsync(int userId, int take = 50);
        Task<int> CountUnreadAsync(int userId);
        Task<Notification?> OpenAsync(int id, int userId);
        Task MarkAllReadAsync(int userId);
        Task NotifyUserAsync(int userId, string title, string message, string? link = null);
        Task NotifyEmployeeAsync(int employeeId, string title, string message, string? link = null);
    }

    public interface IAuditLogFacade
    {
        Task<(List<AuditLog> Items, int Total)> SearchAsync(string? q, int page, int pageSize);
    }
}
