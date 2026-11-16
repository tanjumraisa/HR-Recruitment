using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class FestivalBonusSetupFacade : CrudFacade<FestivalBonusSetup>, IFestivalBonusSetupFacade
    {
        public FestivalBonusSetupFacade(IFestivalBonusSetupRepository repo) : base(repo)
        {
        }

    }
}
