using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetByUserIdAsync(int userId);
        Task<List<Employee>> GetActiveAsync();
        Task<int> GetNextNumberAsync();
    }
}
