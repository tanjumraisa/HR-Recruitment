using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Employee>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.User).Include(x => x.Department).Include(x => x.Designation).Include(x => x.Manager)
                .OrderByDescending(x => x.EmployeeId)
                .ToListAsync();
        }

        public override async Task<Employee?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.User).Include(x => x.Department).Include(x => x.Designation).Include(x => x.Manager)
                .FirstOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking().Include(x => x.User).Include(x => x.Department)
                .Include(x => x.Designation).FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<List<Employee>> GetActiveAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.User)
                .Where(x => x.Status == EmployeeStatus.Active).ToListAsync();
        }

        public async Task<int> GetNextNumberAsync()
        {
            return (await _dbSet.IgnoreQueryFilters().MaxAsync(x => (int?)x.EmployeeId) ?? 0) + 1;
        }
    }
}
