using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class InterviewRepository : Repository<Interview>, IInterviewRepository
    {
        public InterviewRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Interview>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Application).ThenInclude(a => a.Candidate).Include(x => x.Application).ThenInclude(a => a.JobOpening).Include(x => x.Interviewer).ThenInclude(e => e.User)
                .OrderByDescending(x => x.ScheduledAt)
                .ToListAsync();
        }

        public override async Task<Interview?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Application).ThenInclude(a => a.Candidate).Include(x => x.Application).ThenInclude(a => a.JobOpening).Include(x => x.Interviewer).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(x => x.InterviewId == id);
        }

        public async Task<bool> HasConflictAsync(int interviewerId, DateTime scheduledAt, int excludeId)
        {
            var from = scheduledAt.AddHours(-1);
            var to = scheduledAt.AddHours(1);
            return await _dbSet.AnyAsync(x => x.InterviewerId == interviewerId
                && x.Status == InterviewStatus.Scheduled
                && x.InterviewId != excludeId
                && x.ScheduledAt > from && x.ScheduledAt < to);
        }
    }
}
