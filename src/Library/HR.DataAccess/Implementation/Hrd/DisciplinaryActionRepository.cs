using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class DisciplinaryActionRepository : Repository<DisciplinaryAction>, IDisciplinaryActionRepository
    {
        public DisciplinaryActionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
