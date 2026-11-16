using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class BonusRepository : Repository<Bonus>, IBonusRepository
    {
        public BonusRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
