using System.ComponentModel.DataAnnotations;
using System.Reflection;
using HR.DataAccess.Interface;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.Facade.Implementation
{
    // Base business layer for the HRMS modules.
    // Every module gets: data annotation checks, date range checks, duplicate checks,
    // friendly database errors and soft delete (through the generic repository).
    public abstract class CrudFacade<TEntity> : ICrudFacade<TEntity> where TEntity : class
    {
        private static readonly (string From, string To)[] DateRanges =
        {
            ("FromDate", "ToDate"), ("StartDate", "EndDate"), ("IssueDate", "ExpiryDate"), ("PostedOn", "ExpiresOn"), ("WorkedOn", "ExpiryDate")
        };

        protected readonly IRepository<TEntity> Repo;

        protected CrudFacade(IRepository<TEntity> repo)
        {
            Repo = repo;
        }

        // Name column that must be unique (master tables), e.g. Branch.Name.
        protected virtual string? UniqueProperty => null;

        // Column combination that must be unique (link tables), e.g. EmployeeId + SkillId.
        protected virtual string[] UniqueKeys => Array.Empty<string>();

        protected static PropertyInfo KeyProperty { get; } = typeof(TEntity).GetProperty(typeof(TEntity).Name + "Id")!;
        protected static int KeyOf(TEntity e) => (int)KeyProperty.GetValue(e)!;

        public virtual async Task<List<TEntity>> GetAllAsync() =>
            (await Repo.GetAllAsync()).OrderByDescending(KeyOf).ToList();

        public virtual Task<TEntity?> GetByIdAsync(int id) => Repo.GetByIdAsync(id);

        public virtual async Task AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            Normalize(entity);
            await ValidateCommonAsync(entity);
            await ValidateAsync(entity, null);
            await SaveAsync(() => Repo.AddAsync(entity));
            await AfterAddAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var existing = await Repo.GetByIdAsync(KeyOf(entity));
            if (existing == null) return false;

            Normalize(entity);
            await ValidateCommonAsync(entity);
            await ValidateAsync(entity, existing);
            await SaveAsync(() => Repo.UpdateAsync(entity));
            await AfterUpdateAsync(entity, existing);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var existing = await Repo.GetByIdAsync(id);
            if (existing == null) return false;
            await BeforeDeleteAsync(existing);
            var ok = false;
            await SaveAsync(async () => ok = await Repo.DeleteAsync(id));
            return ok;
        }

        // Module specific rules.
        protected virtual Task ValidateAsync(TEntity entity, TEntity? existing) => Task.CompletedTask;
        protected virtual Task AfterAddAsync(TEntity entity) => Task.CompletedTask;
        protected virtual Task AfterUpdateAsync(TEntity entity, TEntity existing) => Task.CompletedTask;
        protected virtual Task BeforeDeleteAsync(TEntity existing) => Task.CompletedTask;

        protected static string? StatusOf(TEntity e) => typeof(TEntity).GetProperty("Status")?.GetValue(e) as string;

        private static void Normalize(TEntity entity)
        {
            foreach (var p in typeof(TEntity).GetProperties().Where(p => p.PropertyType == typeof(string) && p.CanWrite))
            {
                if (p.GetValue(entity) is string s) p.SetValue(entity, string.IsNullOrWhiteSpace(s) && IsNullable(p) ? null : s.Trim());
            }
        }

        private static bool IsNullable(PropertyInfo p) =>
            new NullabilityInfoContext().Create(p).WriteState == NullabilityState.Nullable;

        private async Task ValidateCommonAsync(TEntity entity)
        {
            // [Required], [StringLength], [Range] ...
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(entity, new ValidationContext(entity), results, true))
                throw new BusinessException(results[0].ErrorMessage ?? "Invalid data.");

            // [Choices]
            foreach (var p in typeof(TEntity).GetProperties())
            {
                var choices = p.GetCustomAttribute<ChoicesAttribute>();
                if (choices != null && p.GetValue(entity) is string v && !choices.Values.Contains(v))
                    throw new BusinessException($"Invalid value '{v}' for {LabelOf(p)}.");
            }

            // From / To dates
            foreach (var (fromName, toName) in DateRanges)
            {
                var from = typeof(TEntity).GetProperty(fromName)?.GetValue(entity) as DateTime?;
                var to = typeof(TEntity).GetProperty(toName)?.GetValue(entity) as DateTime?;
                if (from.HasValue && to.HasValue && to.Value.Date < from.Value.Date)
                    throw new BusinessException($"{LabelOf(typeof(TEntity).GetProperty(toName)!)} cannot be before {LabelOf(typeof(TEntity).GetProperty(fromName)!)}.");
            }

            if (UniqueProperty == null && UniqueKeys.Length == 0) return;

            var all = await Repo.GetAllAsync();
            var id = KeyOf(entity);
            if (UniqueProperty != null)
            {
                var prop = typeof(TEntity).GetProperty(UniqueProperty)!;
                var value = prop.GetValue(entity)?.ToString();
                if (all.Any(x => KeyOf(x) != id && string.Equals(prop.GetValue(x)?.ToString(), value, StringComparison.OrdinalIgnoreCase)))
                    throw new BusinessException($"A record with {LabelOf(prop).ToLower()} '{value}' already exists.");
            }
            if (UniqueKeys.Length > 0)
            {
                var props = UniqueKeys.Select(k => typeof(TEntity).GetProperty(k)!).ToList();
                if (all.Any(x => KeyOf(x) != id && props.All(p => Equals(p.GetValue(x), p.GetValue(entity)))))
                    throw new BusinessException("This combination (" + string.Join(", ", props.Select(LabelOf)) + ") already exists.");
            }
        }

        protected static string LabelOf(PropertyInfo p) =>
            p.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? p.Name;

        private static async Task SaveAsync(Func<Task> work)
        {
            try
            {
                await work();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || msg.Contains("UNIQUE", StringComparison.Ordinal))
                    throw new BusinessException("This record already exists (duplicate value).");
                if (msg.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) || msg.Contains("REFERENCE", StringComparison.Ordinal))
                    throw new BusinessException("A selected related record does not exist, or this record is used by other data.");
                throw new BusinessException("The record could not be saved: " + msg);
            }
        }
    }
}
