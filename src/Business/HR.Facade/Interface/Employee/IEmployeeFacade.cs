using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IEmployeeFacade
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee entity);
        Task<bool> UpdateEmployeeAsync(Employee entity);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<Employee?> GetEmployeeByUserIdAsync(int userId);
    }
}
