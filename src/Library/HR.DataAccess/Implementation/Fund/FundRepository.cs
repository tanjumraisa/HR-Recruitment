using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FundRepository : Repository<Fund>, IFundRepository
    {
        public FundRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
