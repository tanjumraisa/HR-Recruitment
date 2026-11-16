using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeShiftFacade : CrudFacade<EmployeeShift>, IEmployeeShiftFacade
    {
        public EmployeeShiftFacade(IEmployeeShiftRepository repo) : base(repo)
        {
        }

    }
}
