using System.Linq.Expressions;
using System.Reflection;
using HR.DataAccess.Interface;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        // Entities are returned untracked, so a modified copy can be saved with UpdateAsync.
        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null) _dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            // Stop tracking, so the same row can be loaded and saved again in the same request.
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            // Only this entity is updated (navigation objects are not touched).
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        // Soft delete for entities that implement ISoftDelete (flag + date), real delete for the others.
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var key = _dbContext.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties[0].Name;
            var entity = await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, key) == id);
            if (entity == null) return false;

            if (entity is ISoftDelete soft)
            {
                if (await HasActiveDependentsAsync(id))
                    throw new BusinessException("This record is used by other active records and cannot be deleted.");

                soft.IsDeleted = true;
                soft.DeletedAt = DateTime.UtcNow;
            }
            else
            {
                _dbSet.Remove(entity);
            }
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
            return true;
        }

        // True when any non-deleted row of another table still points at this record.
        private async Task<bool> HasActiveDependentsAsync(int id)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(TEntity))!;
            var method = typeof(Repository<TEntity>).GetMethod(nameof(AnyDependentAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

            foreach (var fk in entityType.GetReferencingForeignKeys())
            {
                var dependentType = fk.DeclaringEntityType.ClrType;
                var task = (Task<bool>)method.MakeGenericMethod(dependentType)
                    .Invoke(this, new object[] { fk.Properties[0].Name, id })!;
                if (await task) return true;
            }
            return false;
        }

        private async Task<bool> AnyDependentAsync<TDependent>(string foreignKeyProperty, int id) where TDependent : class
        {
            var parameter = Expression.Parameter(typeof(TDependent), "d");
            var left = Expression.Convert(Expression.Property(parameter, foreignKeyProperty), typeof(int?));
            var body = Expression.Equal(left, Expression.Constant(id, typeof(int?)));
            var predicate = Expression.Lambda<Func<TDependent, bool>>(body, parameter);
            return await _dbContext.Set<TDependent>().AnyAsync(predicate);
        }
    }
}