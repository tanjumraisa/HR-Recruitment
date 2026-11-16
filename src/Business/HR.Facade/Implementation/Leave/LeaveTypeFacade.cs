using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class LeaveTypeFacade : ILeaveTypeFacade
    {
        private readonly ILeaveTypeRepository _repo;
        

        public LeaveTypeFacade(ILeaveTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LeaveType>> GetLeaveTypesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<LeaveType?> GetLeaveTypeByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddLeaveTypeAsync(LeaveType entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateLeaveTypeAsync(LeaveType entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.LeaveTypeId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteLeaveTypeAsync(int id)
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

        private async Task ValidateAsync(LeaveType e, LeaveType? existing)
        {
            if (string.IsNullOrWhiteSpace(e.Name)) throw new BusinessException("Leave type name is required.");
            if (e.DaysPerYear < 1 || e.DaysPerYear > 365) throw new BusinessException("Days per year must be between 1 and 365.");
            e.Name = e.Name.Trim();
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.LeaveTypeId != e.LeaveTypeId && x.Name.Equals(e.Name, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessException("A leave type with this name already exists.");
        }
    }
}
