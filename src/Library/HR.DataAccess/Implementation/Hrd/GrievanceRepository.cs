using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class GrievanceRepository : Repository<Grievance>, IGrievanceRepository
    {
        public GrievanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
