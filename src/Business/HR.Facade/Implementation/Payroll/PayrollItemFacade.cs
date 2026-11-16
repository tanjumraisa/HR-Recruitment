using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class PayrollItemFacade : CrudFacade<PayrollItem>, IPayrollItemFacade
    {
        public PayrollItemFacade(IPayrollItemRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(PayrollItem.PayrollId), nameof(PayrollItem.SalaryComponentId) };
    }
}
