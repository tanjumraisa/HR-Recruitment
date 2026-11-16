using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class JobApplicationFacade : IJobApplicationFacade
    {
        private readonly IJobApplicationRepository _repo;
        private readonly IJobOpeningRepository _jobRepo;
        private readonly ICandidateRepository _candidateRepo;
        

        public JobApplicationFacade(IJobApplicationRepository repo, IJobOpeningRepository jobRepo, ICandidateRepository candidateRepo)
        {
            _repo = repo;
            _jobRepo = jobRepo;
            _candidateRepo = candidateRepo;
        }

        public async Task<List<JobApplication>> GetJobApplicationsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<JobApplication?> GetJobApplicationByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddJobApplicationAsync(JobApplication entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.Status = ApplicationStatus.Applied; e.AppliedAt = DateTime.UtcNow;
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateJobApplicationAsync(JobApplication entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.ApplicationId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.AppliedAt = existing.AppliedAt;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteJobApplicationAsync(int id)
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

        private async Task ValidateAsync(JobApplication e, JobApplication? existing)
        {
            var job = await _jobRepo.GetByIdAsync(e.JobOpeningId) ?? throw new BusinessException("Job opening not found.");
            if (await _candidateRepo.GetByIdAsync(e.CandidateId) == null) throw new BusinessException("Candidate not found.");
            if (!ApplicationStatus.All.Contains(e.Status)) throw new BusinessException("Invalid application status.");

            if (existing == null)
            {
                if (job.Status != JobStatus.Open) throw new BusinessException("This job is not open for applications.");
                if (job.Deadline.Date < DateTime.Today) throw new BusinessException("The application deadline has passed.");
                if (await _repo.ExistsAsync(e.JobOpeningId, e.CandidateId)) throw new BusinessException("This candidate has already applied for this job.");
            }
            else if (!ApplicationStatus.CanMove(existing.Status, e.Status))
            {
                throw new BusinessException($"Cannot move an application from {existing.Status} to {e.Status}.");
            }
        }

        public async Task<JobApplication> ApplyAsync(int jobOpeningId, int candidateId, string? coverLetter)
        {
            var application = new JobApplication
            {
                JobOpeningId = jobOpeningId,
                CandidateId = candidateId,
                CoverLetter = coverLetter
            };
            await AddJobApplicationAsync(application);
            return application;
        }

        // Manual moves are limited to Shortlisted / Rejected.
        // Interview, Offered and Hired are set automatically by the Interview and Offer workflows.
        public async Task ChangeStatusAsync(int applicationId, string newStatus)
        {
            var app = await _repo.GetByIdAsync(applicationId) ?? throw new BusinessException("Application not found.");
            if (newStatus != ApplicationStatus.Shortlisted && newStatus != ApplicationStatus.Rejected)
                throw new BusinessException("Only Shortlisted or Rejected can be set manually.");
            if (!ApplicationStatus.CanMove(app.Status, newStatus))
                throw new BusinessException($"Cannot move an application from {app.Status} to {newStatus}.");

            app.Status = newStatus;
            await _repo.UpdateAsync(app);
        }

        public async Task<List<JobApplication>> GetByCandidateAsync(int candidateId)
        {
            return await _repo.GetByCandidateAsync(candidateId);
        }
    }
}
