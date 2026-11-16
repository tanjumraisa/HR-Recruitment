using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class EmploymentTypeRepository : Repository<EmploymentType>, IEmploymentTypeRepository
    {
        public EmploymentTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
