using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeEducationRepository : Repository<EmployeeEducation>, IEmployeeEducationRepository
    {
        public EmployeeEducationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
