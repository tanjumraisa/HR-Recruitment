using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class BonusFacade : CrudFacade<Bonus>, IBonusFacade
    {
        public BonusFacade(IBonusRepository repo) : base(repo)
        {
        }

    }
}
