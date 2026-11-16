using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
    {
        public JobApplicationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<JobApplication>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.JobOpening).Include(x => x.Candidate)
                .OrderByDescending(x => x.ApplicationId)
                .ToListAsync();
        }

        public override async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.JobOpening).Include(x => x.Candidate)
                .FirstOrDefaultAsync(x => x.ApplicationId == id);
        }

        public async Task<List<JobApplication>> GetByCandidateAsync(int candidateId)
        {
            return await _dbSet.AsNoTracking().Include(x => x.JobOpening).Include(x => x.Candidate)
                .Where(x => x.CandidateId == candidateId)
                .OrderByDescending(x => x.ApplicationId).ToListAsync();
        }

        public async Task<bool> ExistsAsync(int jobOpeningId, int candidateId)
        {
            return await _dbSet.AnyAsync(x => x.JobOpeningId == jobOpeningId && x.CandidateId == candidateId);
        }
    }
}
