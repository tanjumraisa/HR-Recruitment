using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class OutsourcedEmployeeRepository : Repository<OutsourcedEmployee>, IOutsourcedEmployeeRepository
    {
        public OutsourcedEmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
