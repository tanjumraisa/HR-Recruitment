using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IPayrollRepository : IRepository<Payroll>
    {
        Task<bool> ExistsAsync(int employeeId, int month, int year, int excludeId);
    }
}
