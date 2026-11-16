using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class LeaveRequestFacade : ILeaveRequestFacade
    {
        private readonly ILeaveRequestRepository _repo;
        private readonly ILeaveTypeRepository _typeRepo;
        

        public LeaveRequestFacade(ILeaveRequestRepository repo, ILeaveTypeRepository typeRepo)
        {
            _repo = repo;
            _typeRepo = typeRepo;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddLeaveRequestAsync(LeaveRequest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.Status = LeaveStatus.Pending; e.ApprovedById = null; e.AppliedAt = DateTime.UtcNow;
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateLeaveRequestAsync(LeaveRequest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.LeaveRequestId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.AppliedAt = existing.AppliedAt; e.ApprovedById = existing.ApprovedById;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteLeaveRequestAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                throw new BusinessException("This record is used by other data and cannot be deleted.");
            }
        }

        private async Task ValidateAsync(LeaveRequest e, LeaveRequest? existing)
        {
            var type = await _typeRepo.GetByIdAsync(e.LeaveTypeId) ?? throw new BusinessException("Leave type not found.");
            if (e.ToDate.Date < e.FromDate.Date) throw new BusinessException("End date cannot be before start date.");
            if (existing == null && e.FromDate.Date < DateTime.Today) throw new BusinessException("Leave cannot start in the past.");

            e.FromDate = e.FromDate.Date;
            e.ToDate = e.ToDate.Date;
            e.TotalDays = (e.ToDate - e.FromDate).Days + 1;

            if (await _repo.HasOverlapAsync(e.EmployeeId, e.FromDate, e.ToDate, e.LeaveRequestId))
                throw new BusinessException("This leave overlaps with another pending or approved leave.");

            var used = await _repo.GetUsedDaysAsync(e.EmployeeId, e.LeaveTypeId, e.FromDate.Year, e.LeaveRequestId);
            if (used + e.TotalDays > type.DaysPerYear)
                throw new BusinessException($"Leave balance exceeded: {type.Name} allows {type.DaysPerYear} days per year, {used} already used or pending.");
        }

        public async Task DecideAsync(int leaveRequestId, bool approve, int? approverEmployeeId)
        {
            var leave = await _repo.GetByIdAsync(leaveRequestId) ?? throw new BusinessException("Leave request not found.");
            if (leave.Status != LeaveStatus.Pending) throw new BusinessException("Only pending requests can be approved or rejected.");
            if (approverEmployeeId.HasValue && approverEmployeeId.Value == leave.EmployeeId)
                throw new BusinessException("You cannot approve your own leave.");

            leave.Status = approve ? LeaveStatus.Approved : LeaveStatus.Rejected;
            leave.ApprovedById = approverEmployeeId;
            await _repo.UpdateAsync(leave);
        }
    }
}
