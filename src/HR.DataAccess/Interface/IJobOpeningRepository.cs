using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IJobOpeningRepository : IRepository<JobOpening>
    {
        Task<List<JobOpening>> GetOpenAsync(string? search);
    }
}
