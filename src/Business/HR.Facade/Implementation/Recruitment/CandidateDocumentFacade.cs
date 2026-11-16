using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CandidateDocumentFacade : CrudFacade<CandidateDocument>, ICandidateDocumentFacade
    {
        public CandidateDocumentFacade(ICandidateDocumentRepository repo) : base(repo)
        {
        }

    }
}
