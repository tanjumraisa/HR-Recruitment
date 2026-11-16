using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CandidateSkillFacade : CrudFacade<CandidateSkill>, ICandidateSkillFacade
    {
        public CandidateSkillFacade(ICandidateSkillRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(CandidateSkill.CandidateId), nameof(CandidateSkill.SkillId) };
    }
}
