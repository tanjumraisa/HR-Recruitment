using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class LeavePolicyFacade : CrudFacade<LeavePolicy>, ILeavePolicyFacade
    {
        public LeavePolicyFacade(ILeavePolicyRepository repo) : base(repo)
        {
        }

    }
}
