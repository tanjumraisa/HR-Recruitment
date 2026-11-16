using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CandidateExperienceFacade : CrudFacade<CandidateExperience>, ICandidateExperienceFacade
    {
        public CandidateExperienceFacade(ICandidateExperienceRepository repo) : base(repo)
        {
        }

    }
}
