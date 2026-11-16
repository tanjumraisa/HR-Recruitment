using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LoanAdvanceRepository : Repository<LoanAdvance>, ILoanAdvanceRepository
    {
        public LoanAdvanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
