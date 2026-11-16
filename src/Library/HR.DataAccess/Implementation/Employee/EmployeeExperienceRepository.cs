using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeExperienceRepository : Repository<EmployeeExperience>, IEmployeeExperienceRepository
    {
        public EmployeeExperienceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
