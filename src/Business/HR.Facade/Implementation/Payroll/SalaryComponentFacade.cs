using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class SalaryComponentFacade : CrudFacade<SalaryComponent>, ISalaryComponentFacade
    {
        public SalaryComponentFacade(ISalaryComponentRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(SalaryComponent.Name);
    }
}
