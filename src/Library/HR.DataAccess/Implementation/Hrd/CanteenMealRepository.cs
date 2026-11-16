using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CanteenMealRepository : Repository<CanteenMeal>, ICanteenMealRepository
    {
        public CanteenMealRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
