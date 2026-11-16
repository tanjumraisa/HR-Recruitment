using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IRoleFacade
    {
        Task<List<Role>> GetRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task AddRoleAsync(Role entity);
        Task<bool> UpdateRoleAsync(Role entity);
        Task<bool> DeleteRoleAsync(int id);

    }
}
