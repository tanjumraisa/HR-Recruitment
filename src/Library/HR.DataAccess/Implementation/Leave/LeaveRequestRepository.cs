using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User).Include(x => x.LeaveType).Include(x => x.ApprovedBy).ThenInclude(e => e!.User)
                .OrderByDescending(x => x.LeaveRequestId)
                .ToListAsync();
        }

        public override async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User).Include(x => x.LeaveType).Include(x => x.ApprovedBy).ThenInclude(e => e!.User)
                .FirstOrDefaultAsync(x => x.LeaveRequestId == id);
        }

        public async Task<bool> HasOverlapAsync(int employeeId, DateTime from, DateTime to, int excludeId)
        {
            return await _dbSet.AnyAsync(x => x.EmployeeId == employeeId
                && x.LeaveRequestId != excludeId
                && (x.Status == LeaveStatus.Pending || x.Status == LeaveStatus.Approved)
                && x.FromDate <= to && x.ToDate >= from);
        }

        public async Task<int> GetUsedDaysAsync(int employeeId, int leaveTypeId, int year, int excludeId)
        {
            return await _dbSet.Where(x => x.EmployeeId == employeeId
                    && x.LeaveTypeId == leaveTypeId
                    && x.LeaveRequestId != excludeId
                    && x.FromDate.Year == year
                    && (x.Status == LeaveStatus.Pending || x.Status == LeaveStatus.Approved))
                .SumAsync(x => (int?)x.TotalDays) ?? 0;
        }
    }
}
