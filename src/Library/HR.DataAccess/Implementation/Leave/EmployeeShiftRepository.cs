using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeShiftRepository : Repository<EmployeeShift>, IEmployeeShiftRepository
    {
        public EmployeeShiftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
