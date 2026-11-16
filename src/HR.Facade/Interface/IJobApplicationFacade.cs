using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IJobApplicationFacade
    {
        Task<List<JobApplication>> GetJobApplicationsAsync();
        Task<JobApplication?> GetJobApplicationByIdAsync(int id);
        Task AddJobApplicationAsync(JobApplication entity);
        Task<bool> UpdateJobApplicationAsync(JobApplication entity);
        Task<bool> DeleteJobApplicationAsync(int id);
        Task<JobApplication> ApplyAsync(int jobOpeningId, int candidateId, string? coverLetter);
        Task ChangeStatusAsync(int applicationId, string newStatus);
        Task<List<JobApplication>> GetByCandidateAsync(int candidateId);
    }
}
