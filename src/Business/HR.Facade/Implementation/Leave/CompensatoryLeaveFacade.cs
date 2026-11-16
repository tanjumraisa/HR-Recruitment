using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class CompensatoryLeaveFacade : CrudFacade<CompensatoryLeave>, ICompensatoryLeaveFacade
    {
        public CompensatoryLeaveFacade(ICompensatoryLeaveRepository repo) : base(repo)
        {
        }

    }
}
