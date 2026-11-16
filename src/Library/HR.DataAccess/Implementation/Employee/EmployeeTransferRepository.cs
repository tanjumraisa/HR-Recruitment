using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeTransferRepository : Repository<EmployeeTransfer>, IEmployeeTransferRepository
    {
        public EmployeeTransferRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
