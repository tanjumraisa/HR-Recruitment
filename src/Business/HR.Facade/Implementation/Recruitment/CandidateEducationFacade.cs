using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CandidateEducationFacade : CrudFacade<CandidateEducation>, ICandidateEducationFacade
    {
        public CandidateEducationFacade(ICandidateEducationRepository repo) : base(repo)
        {
        }

    }
}
