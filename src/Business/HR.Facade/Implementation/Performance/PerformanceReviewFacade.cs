using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class PerformanceReviewFacade : IPerformanceReviewFacade
    {
        private readonly IPerformanceReviewRepository _repo;
        

        public PerformanceReviewFacade(IPerformanceReviewRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PerformanceReview>> GetPerformanceReviewsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<PerformanceReview?> GetPerformanceReviewByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddPerformanceReviewAsync(PerformanceReview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdatePerformanceReviewAsync(PerformanceReview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.ReviewId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.ReviewedAt = existing.ReviewedAt;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeletePerformanceReviewAsync(int id)
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

        private async Task ValidateAsync(PerformanceReview e, PerformanceReview? existing)
        {
            if (e.Rating < 1 || e.Rating > 5) throw new BusinessException("Rating must be between 1 and 5.");
            if (string.IsNullOrWhiteSpace(e.Period)) throw new BusinessException("Review period is required.");
            if (e.EmployeeId == e.ReviewerId) throw new BusinessException("An employee cannot review themselves.");
            await Task.CompletedTask;
        }
    }
}
