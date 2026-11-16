using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class LabourCasesController : CrudController<LabourCase>
    {
        public LabourCasesController(ILabourCaseFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
