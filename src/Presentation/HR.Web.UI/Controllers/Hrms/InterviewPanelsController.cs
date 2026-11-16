using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.Staff)]
    public class InterviewPanelsController : CrudController<InterviewPanel>
    {
        public InterviewPanelsController(IInterviewPanelFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
