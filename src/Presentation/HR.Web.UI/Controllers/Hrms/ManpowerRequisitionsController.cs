using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.Staff)]
    public class ManpowerRequisitionsController : CrudController<ManpowerRequisition>
    {
        public ManpowerRequisitionsController(IManpowerRequisitionFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
