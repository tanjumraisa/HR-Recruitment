using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class PayrollFacade : IPayrollFacade
    {
        private readonly IPayrollRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;
        

        public PayrollFacade(IPayrollRepository repo, IEmployeeRepository employeeRepo)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
        }

        public async Task<List<Payroll>> GetPayrollsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Payroll?> GetPayrollByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddPayrollAsync(Payroll entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.GeneratedAt = DateTime.UtcNow;
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdatePayrollAsync(Payroll entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.PayrollId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.GeneratedAt = existing.GeneratedAt;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeletePayrollAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                throw new BusinessException("This record is used by other data and cannot be deleted.");
            }
        }

        private async Task ValidateAsync(Payroll e, Payroll? existing)
        {
            if (await _employeeRepo.GetByIdAsync(e.EmployeeId) == null) throw new BusinessException("Employee not found.");
            if (e.Month < 1 || e.Month > 12) throw new BusinessException("Month must be between 1 and 12.");
            if (e.Year < 2000 || e.Year > 2100) throw new BusinessException("Year is invalid.");
            if (e.BasicSalary < 0 || e.Allowance < 0 || e.Deduction < 0) throw new BusinessException("Amounts cannot be negative.");
            e.NetSalary = e.BasicSalary + e.Allowance - e.Deduction;
            if (e.NetSalary < 0) throw new BusinessException("Net salary cannot be negative.");
            if (await _repo.ExistsAsync(e.EmployeeId, e.Month, e.Year, e.PayrollId))
                throw new BusinessException("Payroll for this employee and month already exists.");
        }

        // Creates a payroll row for every active employee that does not have one yet.
        public async Task<int> GenerateForAllAsync(int month, int year)
        {
            var created = 0;
            foreach (var emp in await _employeeRepo.GetActiveAsync())
            {
                if (await _repo.ExistsAsync(emp.EmployeeId, month, year, 0)) continue;

                await AddPayrollAsync(new Payroll
                {
                    EmployeeId = emp.EmployeeId,
                    Month = month,
                    Year = year,
                    BasicSalary = emp.Salary,
                    Allowance = Math.Round(emp.Salary * PayrollRules.AllowancePercent / 100m, 2),
                    Deduction = Math.Round(emp.Salary * PayrollRules.DeductionPercent / 100m, 2)
                });
                created++;
            }
            return created;
        }

        public async Task MarkPaidAsync(int payrollId)
        {
            var payroll = await _repo.GetByIdAsync(payrollId) ?? throw new BusinessException("Payroll not found.");
            if (payroll.Status == PayrollStatus.Paid) throw new BusinessException("Already marked as paid.");
            payroll.Status = PayrollStatus.Paid;
            await _repo.UpdateAsync(payroll);
        }
    }
}
