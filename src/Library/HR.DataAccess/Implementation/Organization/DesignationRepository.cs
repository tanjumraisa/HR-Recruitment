using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class DesignationRepository : Repository<Designation>, IDesignationRepository
    {
        public DesignationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Designation>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        public override async Task<Designation?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DesignationId == id);
        }
    }
}
