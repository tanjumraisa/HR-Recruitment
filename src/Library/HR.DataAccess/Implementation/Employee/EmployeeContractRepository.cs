using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeContractRepository : Repository<EmployeeContract>, IEmployeeContractRepository
    {
        public EmployeeContractRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
