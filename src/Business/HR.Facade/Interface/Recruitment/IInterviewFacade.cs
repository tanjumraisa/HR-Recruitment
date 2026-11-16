using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IInterviewFacade
    {
        Task<List<Interview>> GetInterviewsAsync();
        Task<Interview?> GetInterviewByIdAsync(int id);
        Task AddInterviewAsync(Interview entity);
        Task<bool> UpdateInterviewAsync(Interview entity);
        Task<bool> DeleteInterviewAsync(int id);

    }
}
