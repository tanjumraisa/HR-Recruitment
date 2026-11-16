using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Attendance>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public override async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(x => x.AttendanceId == id);
        }

        public async Task<Attendance?> GetByEmployeeDateAsync(int employeeId, DateTime date)
        {
            var d = date.Date;
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == d);
        }
    }
}
