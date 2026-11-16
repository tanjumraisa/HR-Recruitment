using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class JobOpeningRepository : Repository<JobOpening>, IJobOpeningRepository
    {
        public JobOpeningRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<JobOpening>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Department).Include(x => x.Designation)
                .OrderByDescending(x => x.JobOpeningId)
                .ToListAsync();
        }

        public override async Task<JobOpening?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Department).Include(x => x.Designation)
                .FirstOrDefaultAsync(x => x.JobOpeningId == id);
        }

        public async Task<List<JobOpening>> GetOpenAsync(string? search)
        {
            var today = DateTime.Today;
            var query = _dbSet.AsNoTracking().Include(x => x.Department).Include(x => x.Designation)
                .Where(x => x.Status == JobStatus.Open && x.Deadline >= today);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Title.Contains(search) || x.Department.Name.Contains(search));

            return await query.OrderBy(x => x.Deadline).ToListAsync();
        }
    }
}
