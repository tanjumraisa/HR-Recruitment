using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class HseIncidentsController : CrudController<HseIncident>
    {
        public HseIncidentsController(IHseIncidentFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
