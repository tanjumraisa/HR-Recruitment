using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class WorkFromHomeRequestRepository : Repository<WorkFromHomeRequest>, IWorkFromHomeRequestRepository
    {
        public WorkFromHomeRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
