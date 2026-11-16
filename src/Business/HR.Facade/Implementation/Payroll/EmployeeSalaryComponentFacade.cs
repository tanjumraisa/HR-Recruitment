using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeSalaryComponentFacade : CrudFacade<EmployeeSalaryComponent>, IEmployeeSalaryComponentFacade
    {
        public EmployeeSalaryComponentFacade(IEmployeeSalaryComponentRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(EmployeeSalaryComponent.EmployeeId), nameof(EmployeeSalaryComponent.SalaryComponentId) };
    }
}
