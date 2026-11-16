using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        Task<List<JobApplication>> GetByCandidateAsync(int candidateId);
        Task<bool> ExistsAsync(int jobOpeningId, int candidateId);
    }
}
