using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class FundContributionFacade : CrudFacade<FundContribution>, IFundContributionFacade
    {
        public FundContributionFacade(IFundContributionRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(FundContribution.FundMembershipId), nameof(FundContribution.Month), nameof(FundContribution.Year) };
    }
}
