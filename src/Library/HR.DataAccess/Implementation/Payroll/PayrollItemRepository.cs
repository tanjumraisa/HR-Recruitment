using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class PayrollItemRepository : Repository<PayrollItem>, IPayrollItemRepository
    {
        public PayrollItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
