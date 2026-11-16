using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<User>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().Include(x => x.Role)
                .OrderByDescending(x => x.UserId)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking().Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}