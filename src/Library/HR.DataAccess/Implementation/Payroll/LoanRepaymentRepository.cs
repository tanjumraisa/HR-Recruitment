using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LoanRepaymentRepository : Repository<LoanRepayment>, ILoanRepaymentRepository
    {
        public LoanRepaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
