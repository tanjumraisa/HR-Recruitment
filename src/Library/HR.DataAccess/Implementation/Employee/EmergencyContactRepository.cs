using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmergencyContactRepository : Repository<EmergencyContact>, IEmergencyContactRepository
    {
        public EmergencyContactRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
