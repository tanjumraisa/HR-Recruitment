using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmployeePromotionRepository : Repository<EmployeePromotion>, IEmployeePromotionRepository
    {
        public EmployeePromotionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
