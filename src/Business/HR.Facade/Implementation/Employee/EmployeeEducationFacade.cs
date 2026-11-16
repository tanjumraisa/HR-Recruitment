using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeEducationFacade : CrudFacade<EmployeeEducation>, IEmployeeEducationFacade
    {
        public EmployeeEducationFacade(IEmployeeEducationRepository repo) : base(repo)
        {
        }

    }
}
