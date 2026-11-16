using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class TravelExpenseRepository : Repository<TravelExpense>, ITravelExpenseRepository
    {
        public TravelExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
