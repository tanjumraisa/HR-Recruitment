using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FundMembershipRepository : Repository<FundMembership>, IFundMembershipRepository
    {
        public FundMembershipRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
