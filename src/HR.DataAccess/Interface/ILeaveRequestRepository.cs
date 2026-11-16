using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<bool> HasOverlapAsync(int employeeId, DateTime from, DateTime to, int excludeId);
        Task<int> GetUsedDaysAsync(int employeeId, int leaveTypeId, int year, int excludeId);
    }
}
