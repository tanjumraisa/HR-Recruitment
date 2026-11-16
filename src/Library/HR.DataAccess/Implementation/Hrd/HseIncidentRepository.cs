using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class HseIncidentRepository : Repository<HseIncident>, IHseIncidentRepository
    {
        public HseIncidentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
