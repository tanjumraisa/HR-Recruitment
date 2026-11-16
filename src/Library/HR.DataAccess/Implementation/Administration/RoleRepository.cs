using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Role>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(x => x.RoleId)
                .ToListAsync();
        }

        public override async Task<Role?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.RoleId == id);
        }
    }
}
