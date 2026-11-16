using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmploymentTypeFacade : CrudFacade<EmploymentType>, IEmploymentTypeFacade
    {
        public EmploymentTypeFacade(IEmploymentTypeRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(EmploymentType.Name);
    }
}
