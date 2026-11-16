using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class ReviewCycleRepository : Repository<ReviewCycle>, IReviewCycleRepository
    {
        public ReviewCycleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
