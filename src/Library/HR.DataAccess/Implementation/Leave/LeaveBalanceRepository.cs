using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
    {
        public LeaveBalanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
