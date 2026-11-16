using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class SalaryComponentRepository : Repository<SalaryComponent>, ISalaryComponentRepository
    {
        public SalaryComponentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
