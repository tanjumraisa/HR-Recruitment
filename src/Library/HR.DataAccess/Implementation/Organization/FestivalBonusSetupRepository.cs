using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FestivalBonusSetupRepository : Repository<FestivalBonusSetup>, IFestivalBonusSetupRepository
    {
        public FestivalBonusSetupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
