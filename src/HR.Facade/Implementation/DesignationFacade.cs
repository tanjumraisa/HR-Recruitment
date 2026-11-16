using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class DesignationFacade : IDesignationFacade
    {
        private readonly IDesignationRepository _repo;
        

        public DesignationFacade(IDesignationRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Designation>> GetDesignationsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Designation?> GetDesignationByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddDesignationAsync(Designation entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateDesignationAsync(Designation entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.DesignationId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteDesignationAsync(int id)
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

        private async Task ValidateAsync(Designation e, Designation? existing)
        {
            if (string.IsNullOrWhiteSpace(e.Title)) throw new BusinessException("Designation title is required.");
            if (e.BaseSalary < 0) throw new BusinessException("Base salary cannot be negative.");
            e.Title = e.Title.Trim();
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.DesignationId != e.DesignationId && x.Title.Equals(e.Title, StringComparison.OrdinalIgnoreCase)))
                throw new BusinessException("A designation with this title already exists.");
        }
    }
}
