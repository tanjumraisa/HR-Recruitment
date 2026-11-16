using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class HrAuditRepository : Repository<HrAudit>, IHrAuditRepository
    {
        public HrAuditRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
