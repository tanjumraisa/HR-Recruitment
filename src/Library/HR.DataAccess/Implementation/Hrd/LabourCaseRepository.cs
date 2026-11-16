using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class LabourCaseRepository : Repository<LabourCase>, ILabourCaseRepository
    {
        public LabourCaseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
