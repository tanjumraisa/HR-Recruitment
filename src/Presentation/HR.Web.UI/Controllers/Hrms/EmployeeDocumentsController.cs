using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authorization;

namespace HR.Web.UI.Controllers.Hrms
{
    [Authorize(Roles = RoleNames.HrAdmin)]
    public class EmployeeDocumentsController : CrudController<EmployeeDocument>
    {
        public EmployeeDocumentsController(IEmployeeDocumentFacade facade, IRecordFacade records) : base(facade, records)
        {
        }
    }
}
