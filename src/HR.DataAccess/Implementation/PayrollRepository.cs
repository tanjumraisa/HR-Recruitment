using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class PayrollRepository : Repository<Payroll>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Payroll>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User)
                .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
                .ToListAsync();
        }

        public override async Task<Payroll?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(x => x.PayrollId == id);
        }

        public async Task<bool> ExistsAsync(int employeeId, int month, int year, int excludeId)
        {
            return await _dbSet.AnyAsync(x => x.EmployeeId == employeeId && x.Month == month
                && x.Year == year && x.PayrollId != excludeId);
        }
    }
}
