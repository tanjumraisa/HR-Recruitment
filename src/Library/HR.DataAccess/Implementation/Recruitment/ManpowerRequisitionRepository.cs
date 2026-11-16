using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class ManpowerRequisitionRepository : Repository<ManpowerRequisition>, IManpowerRequisitionRepository
    {
        public ManpowerRequisitionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
