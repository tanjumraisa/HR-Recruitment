using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class JobOpeningFacade : IJobOpeningFacade
    {
        private readonly IJobOpeningRepository _repo;
        

        public JobOpeningFacade(IJobOpeningRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<JobOpening>> GetJobOpeningsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<JobOpening?> GetJobOpeningByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddJobOpeningAsync(JobOpening entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.CreatedAt = DateTime.UtcNow;
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateJobOpeningAsync(JobOpening entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.JobOpeningId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.PostedById = existing.PostedById; e.CreatedAt = existing.CreatedAt;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteJobOpeningAsync(int id)
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

        private async Task ValidateAsync(JobOpening e, JobOpening? existing)
        {
            if (string.IsNullOrWhiteSpace(e.Title)) throw new BusinessException("Job title is required.");
            if (e.Vacancies < 1) throw new BusinessException("Vacancies must be at least 1.");
            if (e.SalaryMin < 0 || e.SalaryMax < e.SalaryMin) throw new BusinessException("Salary range is invalid (max must be >= min).");
            if (existing == null && e.Deadline.Date < DateTime.Today) throw new BusinessException("Deadline cannot be in the past.");
            if (!JobStatus.All.Contains(e.Status)) throw new BusinessException("Invalid job status.");
        }

        public async Task<List<JobOpening>> GetOpenJobsAsync(string? search)
        {
            return await _repo.GetOpenAsync(search);
        }

        public async Task SetStatusAsync(int id, string status)
        {
            var job = await _repo.GetByIdAsync(id) ?? throw new BusinessException("Job opening not found.");
            if (!JobStatus.All.Contains(status)) throw new BusinessException("Invalid job status.");
            job.Status = status;
            await _repo.UpdateAsync(job);
        }
    }
}
