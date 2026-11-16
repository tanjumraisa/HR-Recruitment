using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class EmployeeShiftsController : CrudController<EmployeeShift>
    {
        public EmployeeShiftsController(IEmployeeShiftFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
