using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class HrBudgetFacade : CrudFacade<HrBudget>, IHrBudgetFacade
    {
        public HrBudgetFacade(IHrBudgetRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(HrBudget.FinancialYearId), nameof(HrBudget.DepartmentId), nameof(HrBudget.Category) };
    }
}
