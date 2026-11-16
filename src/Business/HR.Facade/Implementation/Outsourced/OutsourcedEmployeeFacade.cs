using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class OutsourcedEmployeeFacade : CrudFacade<OutsourcedEmployee>, IOutsourcedEmployeeFacade
    {
        public OutsourcedEmployeeFacade(IOutsourcedEmployeeRepository repo) : base(repo)
        {
        }

    }
}
