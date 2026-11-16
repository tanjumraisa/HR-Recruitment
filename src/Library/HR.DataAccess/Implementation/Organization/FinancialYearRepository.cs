using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class FinancialYearRepository : Repository<FinancialYear>, IFinancialYearRepository
    {
        public FinancialYearRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
