using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CandidateSkillRepository : Repository<CandidateSkill>, ICandidateSkillRepository
    {
        public CandidateSkillRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
