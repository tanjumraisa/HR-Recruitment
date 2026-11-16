using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IPayrollFacade
    {
        Task<List<Payroll>> GetPayrollsAsync();
        Task<Payroll?> GetPayrollByIdAsync(int id);
        Task AddPayrollAsync(Payroll entity);
        Task<bool> UpdatePayrollAsync(Payroll entity);
        Task<bool> DeletePayrollAsync(int id);
        Task<int> GenerateForAllAsync(int month, int year);
        Task MarkPaidAsync(int payrollId);
    }
}
