using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class CandidateFacade : ICandidateFacade
    {
        private readonly ICandidateRepository _repo;
        

        public CandidateFacade(ICandidateRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Candidate>> GetCandidatesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Candidate?> GetCandidateByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddCandidateAsync(Candidate entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateCandidateAsync(Candidate entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.CandidateId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.UserId = existing.UserId; e.CreatedAt = existing.CreatedAt; e.ResumePath = existing.ResumePath; e.ResumeFileName = existing.ResumeFileName;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteCandidateAsync(int id)
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

        private async Task ValidateAsync(Candidate e, Candidate? existing)
        {
            if (string.IsNullOrWhiteSpace(e.FullName)) throw new BusinessException("Candidate name is required.");
            if (string.IsNullOrWhiteSpace(e.Email) || !e.Email.Contains('@')) throw new BusinessException("A valid email is required.");
            if (e.ExperienceYears < 0) throw new BusinessException("Experience cannot be negative.");
            e.Email = e.Email.Trim().ToLowerInvariant();
            var found = await _repo.GetByEmailAsync(e.Email);
            if (found != null && found.CandidateId != e.CandidateId) throw new BusinessException("A candidate with this email already exists.");
        }

        public async Task<Candidate?> GetCandidateByUserIdAsync(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task SetResumeAsync(int candidateId, string storedFileName, string originalFileName)
        {
            var candidate = await _repo.GetByIdAsync(candidateId) ?? throw new BusinessException("Candidate not found.");
            candidate.ResumePath = storedFileName;
            candidate.ResumeFileName = originalFileName;
            await _repo.UpdateAsync(candidate);
        }
    }
}
