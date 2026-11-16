using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeeSkillRepository : Repository<EmployeeSkill>, IEmployeeSkillRepository
    {
        public EmployeeSkillRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
