using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeSkillFacade : CrudFacade<EmployeeSkill>, IEmployeeSkillFacade
    {
        public EmployeeSkillFacade(IEmployeeSkillRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(EmployeeSkill.EmployeeId), nameof(EmployeeSkill.SkillId) };
    }
}
