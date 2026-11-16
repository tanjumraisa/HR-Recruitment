using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class HrAuditFacade : CrudFacade<HrAudit>, IHrAuditFacade
    {
        public HrAuditFacade(IHrAuditRepository repo) : base(repo)
        {
        }

    }
}
