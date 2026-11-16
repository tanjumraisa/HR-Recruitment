using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class RoleFacade : IRoleFacade
    {
        private readonly IRoleRepository _repo;
        

        public RoleFacade(IRoleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddRoleAsync(Role entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateRoleAsync(Role entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.RoleId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteRoleAsync(int id)
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

        private async Task ValidateAsync(Role e, Role? existing)
        {
            await Task.CompletedTask;
        }
    }
}
