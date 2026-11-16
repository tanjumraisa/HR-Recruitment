using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CandidateExperienceRepository : Repository<CandidateExperience>, ICandidateExperienceRepository
    {
        public CandidateExperienceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
