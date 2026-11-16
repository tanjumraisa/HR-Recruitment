using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class PerformanceReviewRepository : Repository<PerformanceReview>, IPerformanceReviewRepository
    {
        public PerformanceReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<PerformanceReview>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User).Include(x => x.Reviewer).ThenInclude(e => e.User)
                .OrderByDescending(x => x.ReviewId)
                .ToListAsync();
        }

        public override async Task<PerformanceReview?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Employee).ThenInclude(e => e.User).Include(x => x.Reviewer).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(x => x.ReviewId == id);
        }
    }
}
