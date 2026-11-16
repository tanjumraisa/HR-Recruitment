using HR.Entities;

namespace HR.DataAccess.Interface
{
    public interface IOfferLetterRepository : IRepository<OfferLetter>
    {
        Task<OfferLetter?> GetByApplicationIdAsync(int applicationId);
    }
}
