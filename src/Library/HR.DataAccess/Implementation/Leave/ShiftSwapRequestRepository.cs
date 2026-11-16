using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class ShiftSwapRequestRepository : Repository<ShiftSwapRequest>, IShiftSwapRequestRepository
    {
        public ShiftSwapRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
