using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IAttendanceFacade
    {
        Task<List<Attendance>> GetAttendancesAsync();
        Task<Attendance?> GetAttendanceByIdAsync(int id);
        Task AddAttendanceAsync(Attendance entity);
        Task<bool> UpdateAttendanceAsync(Attendance entity);
        Task<bool> DeleteAttendanceAsync(int id);
        Task CheckInAsync(int employeeId);
        Task CheckOutAsync(int employeeId);
    }
}
