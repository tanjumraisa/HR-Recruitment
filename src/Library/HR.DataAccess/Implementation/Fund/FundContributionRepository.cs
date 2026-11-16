using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FundContributionRepository : Repository<FundContribution>, IFundContributionRepository
    {
        public FundContributionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
