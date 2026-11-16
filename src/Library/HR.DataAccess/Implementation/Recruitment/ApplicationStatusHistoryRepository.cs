using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class ApplicationStatusHistoryRepository : Repository<ApplicationStatusHistory>, IApplicationStatusHistoryRepository
    {
        public ApplicationStatusHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
