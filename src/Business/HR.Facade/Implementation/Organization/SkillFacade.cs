using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class SkillFacade : CrudFacade<Skill>, ISkillFacade
    {
        public SkillFacade(ISkillRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Skill.Name);
    }
}
