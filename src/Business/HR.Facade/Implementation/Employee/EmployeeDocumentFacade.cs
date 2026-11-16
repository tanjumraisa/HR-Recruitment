using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeDocumentFacade : CrudFacade<EmployeeDocument>, IEmployeeDocumentFacade
    {
        public EmployeeDocumentFacade(IEmployeeDocumentRepository repo) : base(repo)
        {
        }

    }
}
