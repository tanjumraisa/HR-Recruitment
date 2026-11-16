using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class FundFacade : CrudFacade<Fund>, IFundFacade
    {
        public FundFacade(IFundRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Fund.Name);
    }
}
