using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class OvertimeRecordsController : CrudController<OvertimeRecord>
    {
        public OvertimeRecordsController(IOvertimeRecordFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
