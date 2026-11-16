using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class OutsourcingAgencyRepository : Repository<OutsourcingAgency>, IOutsourcingAgencyRepository
    {
        public OutsourcingAgencyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
