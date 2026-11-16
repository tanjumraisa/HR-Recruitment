using HR.Entities;

namespace HR.Facade.Interface
{
    public interface ICandidateFacade
    {
        Task<List<Candidate>> GetCandidatesAsync();
        Task<Candidate?> GetCandidateByIdAsync(int id);
        Task AddCandidateAsync(Candidate entity);
        Task<bool> UpdateCandidateAsync(Candidate entity);
        Task<bool> DeleteCandidateAsync(int id);
        Task<Candidate?> GetCandidateByUserIdAsync(int userId);
        Task SetResumeAsync(int candidateId, string storedFileName, string originalFileName);
    }
}
