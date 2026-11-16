using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class AttendanceCorrectionRepository : Repository<AttendanceCorrection>, IAttendanceCorrectionRepository
    {
        public AttendanceCorrectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
