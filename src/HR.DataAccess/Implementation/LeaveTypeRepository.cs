using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class LeaveTypeRepository : Repository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<LeaveType>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public override async Task<LeaveType?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.LeaveTypeId == id);
        }
    }
}
