using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IOfferLetterFacade
    {
        Task<List<OfferLetter>> GetOfferLettersAsync();
        Task<OfferLetter?> GetOfferLetterByIdAsync(int id);
        Task AddOfferLetterAsync(OfferLetter entity);
        Task<bool> UpdateOfferLetterAsync(OfferLetter entity);
        Task<bool> DeleteOfferLetterAsync(int id);
        Task RespondAsync(int offerId, bool accept);
    }
}
