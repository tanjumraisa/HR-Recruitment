using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class ReviewCycleFacade : CrudFacade<ReviewCycle>, IReviewCycleFacade
    {
        public ReviewCycleFacade(IReviewCycleRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(ReviewCycle.Name);
    }
}
