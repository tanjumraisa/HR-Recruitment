using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class OutsourcedAttendanceRepository : Repository<OutsourcedAttendance>, IOutsourcedAttendanceRepository
    {
        public OutsourcedAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
