using HR.DataAccess.Interface;
using HR.Entities;

namespace HR.DataAccess.Implementation
{
    public class CandidateDocumentRepository : Repository<CandidateDocument>, ICandidateDocumentRepository
    {
        public CandidateDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
