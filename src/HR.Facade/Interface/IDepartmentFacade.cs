using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IDepartmentFacade
    {
        Task<List<Department>> GetDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task AddDepartmentAsync(Department entity);
        Task<bool> UpdateDepartmentAsync(Department entity);
        Task<bool> DeleteDepartmentAsync(int id);

    }
}
