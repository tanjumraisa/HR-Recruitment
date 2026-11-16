using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class ApplicationStatusHistoryFacade : CrudFacade<ApplicationStatusHistory>, IApplicationStatusHistoryFacade
    {
        public ApplicationStatusHistoryFacade(IApplicationStatusHistoryRepository repo) : base(repo)
        {
        }

    }
}
