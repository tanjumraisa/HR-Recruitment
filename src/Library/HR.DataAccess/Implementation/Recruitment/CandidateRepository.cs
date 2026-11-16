using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Candidate>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderByDescending(x => x.CandidateId)
                .ToListAsync();
        }

        public override async Task<Candidate?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CandidateId == id);
        }

        public async Task<Candidate?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
