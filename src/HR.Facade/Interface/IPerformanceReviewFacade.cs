using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IPerformanceReviewFacade
    {
        Task<List<PerformanceReview>> GetPerformanceReviewsAsync();
        Task<PerformanceReview?> GetPerformanceReviewByIdAsync(int id);
        Task AddPerformanceReviewAsync(PerformanceReview entity);
        Task<bool> UpdatePerformanceReviewAsync(PerformanceReview entity);
        Task<bool> DeletePerformanceReviewAsync(int id);

    }
}
