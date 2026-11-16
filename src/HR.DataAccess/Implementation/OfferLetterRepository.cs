using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class OfferLetterRepository : Repository<OfferLetter>, IOfferLetterRepository
    {
        public OfferLetterRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<OfferLetter>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Application).ThenInclude(a => a.Candidate).Include(x => x.Application).ThenInclude(a => a.JobOpening)
                .OrderByDescending(x => x.OfferId)
                .ToListAsync();
        }

        public override async Task<OfferLetter?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Application).ThenInclude(a => a.Candidate).Include(x => x.Application).ThenInclude(a => a.JobOpening)
                .FirstOrDefaultAsync(x => x.OfferId == id);
        }

        public async Task<OfferLetter?> GetByApplicationIdAsync(int applicationId)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.ApplicationId == applicationId);
        }
    }
}
