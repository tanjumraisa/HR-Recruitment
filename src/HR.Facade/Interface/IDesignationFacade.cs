using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IDesignationFacade
    {
        Task<List<Designation>> GetDesignationsAsync();
        Task<Designation?> GetDesignationByIdAsync(int id);
        Task AddDesignationAsync(Designation entity);
        Task<bool> UpdateDesignationAsync(Designation entity);
        Task<bool> DeleteDesignationAsync(int id);

    }
}
