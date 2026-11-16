using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class JobAdvertisementRepository : Repository<JobAdvertisement>, IJobAdvertisementRepository
    {
        public JobAdvertisementRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
