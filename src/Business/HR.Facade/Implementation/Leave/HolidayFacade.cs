using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class HolidayFacade : CrudFacade<Holiday>, IHolidayFacade
    {
        public HolidayFacade(IHolidayRepository repo) : base(repo)
        {
        }

    }
}
