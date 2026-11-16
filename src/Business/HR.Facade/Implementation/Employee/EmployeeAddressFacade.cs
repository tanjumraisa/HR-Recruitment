using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeAddressFacade : CrudFacade<EmployeeAddress>, IEmployeeAddressFacade
    {
        public EmployeeAddressFacade(IEmployeeAddressRepository repo) : base(repo)
        {
        }

    }
}
