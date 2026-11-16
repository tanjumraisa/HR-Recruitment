using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class FinancialYearFacade : CrudFacade<FinancialYear>, IFinancialYearFacade
    {
        public FinancialYearFacade(IFinancialYearRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(FinancialYear.Name);
    }
}
