using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LeaveWithdrawalRepository : Repository<LeaveWithdrawal>, ILeaveWithdrawalRepository
    {
        public LeaveWithdrawalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
