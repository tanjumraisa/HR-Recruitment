using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CanteenItemFacade : CrudFacade<CanteenItem>, ICanteenItemFacade
    {
        public CanteenItemFacade(ICanteenItemRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(CanteenItem.Name);
    }
}
