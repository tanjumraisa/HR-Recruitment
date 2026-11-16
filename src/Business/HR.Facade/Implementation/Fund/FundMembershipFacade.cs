using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class FundMembershipFacade : CrudFacade<FundMembership>, IFundMembershipFacade
    {
        public FundMembershipFacade(IFundMembershipRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(FundMembership.FundId), nameof(FundMembership.EmployeeId) };
    }
}
