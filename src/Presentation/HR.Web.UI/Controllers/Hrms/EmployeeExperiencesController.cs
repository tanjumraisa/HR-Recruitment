using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class EmployeeExperiencesController : CrudController<EmployeeExperience>
    {
        public EmployeeExperiencesController(IEmployeeExperienceFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
