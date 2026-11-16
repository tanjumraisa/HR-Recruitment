using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class GoalRepository : Repository<Goal>, IGoalRepository
    {
        public GoalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
