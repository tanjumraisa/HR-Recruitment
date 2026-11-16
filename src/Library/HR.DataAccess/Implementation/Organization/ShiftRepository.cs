using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
