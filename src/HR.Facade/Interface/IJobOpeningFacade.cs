using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IJobOpeningFacade
    {
        Task<List<JobOpening>> GetJobOpeningsAsync();
        Task<JobOpening?> GetJobOpeningByIdAsync(int id);
        Task AddJobOpeningAsync(JobOpening entity);
        Task<bool> UpdateJobOpeningAsync(JobOpening entity);
        Task<bool> DeleteJobOpeningAsync(int id);
        Task<List<JobOpening>> GetOpenJobsAsync(string? search);
        Task SetStatusAsync(int id, string status);
    }
}
