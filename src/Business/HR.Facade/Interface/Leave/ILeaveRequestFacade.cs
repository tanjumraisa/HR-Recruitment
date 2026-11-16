using HR.Entities;

namespace HR.Facade.Interface
{
    public interface ILeaveRequestFacade
    {
        Task<List<LeaveRequest>> GetLeaveRequestsAsync();
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        Task AddLeaveRequestAsync(LeaveRequest entity);
        Task<bool> UpdateLeaveRequestAsync(LeaveRequest entity);
        Task<bool> DeleteLeaveRequestAsync(int id);
        Task DecideAsync(int leaveRequestId, bool approve, int? approverEmployeeId);
    }
}
