using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance?> GetByEmployeeDateAsync(int employeeId, DateTime date);
    }
}
