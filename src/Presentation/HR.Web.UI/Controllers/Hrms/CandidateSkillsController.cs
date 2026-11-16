using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.Staff)]
    public class CandidateSkillsController : CrudController<CandidateSkill>
    {
        public CandidateSkillsController(ICandidateSkillFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
