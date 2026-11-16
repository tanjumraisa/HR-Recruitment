using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Department>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public override async Task<Department?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DepartmentId == id);
        }
    }
}
