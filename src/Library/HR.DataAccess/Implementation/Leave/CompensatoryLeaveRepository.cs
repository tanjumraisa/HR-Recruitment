using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CompensatoryLeaveRepository : Repository<CompensatoryLeave>, ICompensatoryLeaveRepository
    {
        public CompensatoryLeaveRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
