using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IInterviewRepository : IRepository<Interview>
    {
        Task<bool> HasConflictAsync(int interviewerId, DateTime scheduledAt, int excludeId);
    }
}
