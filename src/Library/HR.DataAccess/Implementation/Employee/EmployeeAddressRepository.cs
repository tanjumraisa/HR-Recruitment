using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeAddressRepository : Repository<EmployeeAddress>, IEmployeeAddressRepository
    {
        public EmployeeAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
