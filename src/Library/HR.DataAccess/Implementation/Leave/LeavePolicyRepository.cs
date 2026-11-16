using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LeavePolicyRepository : Repository<LeavePolicy>, ILeavePolicyRepository
    {
        public LeavePolicyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
