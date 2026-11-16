using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class BonusesController : CrudController<Bonus>
    {
        public BonusesController(IBonusFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
