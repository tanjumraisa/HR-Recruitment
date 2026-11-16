using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class HseIncidentFacade : CrudFacade<HseIncident>, IHseIncidentFacade
    {
        public HseIncidentFacade(IHseIncidentRepository repo) : base(repo)
        {
        }

    }
}
