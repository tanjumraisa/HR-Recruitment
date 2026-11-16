using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class DepartmentFacade : IDepartmentFacade
    {
        private readonly IDepartmentRepository _repo;
        

        public DepartmentFacade(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddDepartmentAsync(Department entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateDepartmentAsync(Department entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.DepartmentId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
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

        private async Task ValidateAsync(Department e, Department? existing)
        {
            if (string.IsNullOrWhiteSpace(e.Name)) throw new BusinessException("Department name is required.");
            e.Name = e.Name.Trim();
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.DepartmentId != e.DepartmentId && x.Name.Equals(e.Name, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessException("A department with this name already exists.");
        }
    }
}
