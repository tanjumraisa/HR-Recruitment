using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class HrBudgetRepository : Repository<HrBudget>, IHrBudgetRepository
    {
        public HrBudgetRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
