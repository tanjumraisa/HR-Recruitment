using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FundWithdrawalRepository : Repository<FundWithdrawal>, IFundWithdrawalRepository
    {
        public FundWithdrawalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
