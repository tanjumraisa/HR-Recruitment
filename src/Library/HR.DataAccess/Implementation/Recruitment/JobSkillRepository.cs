using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class JobSkillRepository : Repository<JobSkill>, IJobSkillRepository
    {
        public JobSkillRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
