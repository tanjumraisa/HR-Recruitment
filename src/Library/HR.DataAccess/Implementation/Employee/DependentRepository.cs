using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class DependentRepository : Repository<Dependent>, IDependentRepository
    {
        public DependentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
