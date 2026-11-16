using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.Staff)]
    public class CandidateExperiencesController : CrudController<CandidateExperience>
    {
        public CandidateExperiencesController(ICandidateExperienceFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
