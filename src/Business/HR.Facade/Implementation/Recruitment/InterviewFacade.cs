using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class InterviewFacade : IInterviewFacade
    {
        private readonly IInterviewRepository _repo;
        private readonly IJobApplicationRepository _appRepo;
        

        public InterviewFacade(IInterviewRepository repo, IJobApplicationRepository appRepo)
        {
            _repo = repo;
            _appRepo = appRepo;
        }

        public async Task<List<Interview>> GetInterviewsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Interview?> GetInterviewByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddInterviewAsync(Interview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);

            var app = await _appRepo.GetByIdAsync(e.ApplicationId);
            if (app != null && app.Status == ApplicationStatus.Shortlisted)
            {
                app.Status = ApplicationStatus.Interview;
                await _appRepo.UpdateAsync(app);
            }
        }

        public async Task<bool> UpdateInterviewAsync(Interview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.InterviewId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteInterviewAsync(int id)
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

        private async Task ValidateAsync(Interview e, Interview? existing)
        {
            var app = await _appRepo.GetByIdAsync(e.ApplicationId) ?? throw new BusinessException("Application not found.");
            if (string.IsNullOrWhiteSpace(e.Mode)) throw new BusinessException("Interview mode is required.");
            if (e.Score.HasValue && (e.Score < 1 || e.Score > 10)) throw new BusinessException("Score must be between 1 and 10.");

            if (existing == null)
            {
                if (app.Status != ApplicationStatus.Shortlisted && app.Status != ApplicationStatus.Interview)
                    throw new BusinessException("Only shortlisted applications can be scheduled for interview.");
                if (e.ScheduledAt <= DateTime.Now) throw new BusinessException("Interview must be scheduled in the future.");
                e.Status = InterviewStatus.Scheduled;
            }
            if (e.Status == InterviewStatus.Scheduled && await _repo.HasConflictAsync(e.InterviewerId, e.ScheduledAt, e.InterviewId))
                throw new BusinessException("The interviewer already has an interview within one hour of this time.");
        }
    }
}
