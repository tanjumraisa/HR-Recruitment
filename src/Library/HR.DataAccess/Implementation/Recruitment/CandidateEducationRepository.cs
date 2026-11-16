using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CandidateEducationRepository : Repository<CandidateEducation>, ICandidateEducationRepository
    {
        public CandidateEducationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
