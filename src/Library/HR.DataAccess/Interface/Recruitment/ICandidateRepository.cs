using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        Task<Candidate?> GetByUserIdAsync(int userId);
        Task<Candidate?> GetByEmailAsync(string email);
    }
}
