using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CanteenItemRepository : Repository<CanteenItem>, ICanteenItemRepository
    {
        public CanteenItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
