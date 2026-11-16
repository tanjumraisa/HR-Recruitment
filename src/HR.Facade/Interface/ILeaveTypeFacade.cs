using HR.Entities;

namespace HR.Facade.Interface
{
    public interface ILeaveTypeFacade
    {
        Task<List<LeaveType>> GetLeaveTypesAsync();
        Task<LeaveType?> GetLeaveTypeByIdAsync(int id);
        Task AddLeaveTypeAsync(LeaveType entity);
        Task<bool> UpdateLeaveTypeAsync(LeaveType entity);
        Task<bool> DeleteLeaveTypeAsync(int id);

    }
}
