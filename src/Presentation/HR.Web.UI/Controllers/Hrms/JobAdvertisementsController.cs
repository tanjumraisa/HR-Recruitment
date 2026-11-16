using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.Staff)]
    public class JobAdvertisementsController : CrudController<JobAdvertisement>
    {
        public JobAdvertisementsController(IJobAdvertisementFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
