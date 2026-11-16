using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class OutsourcingAgencyFacade : CrudFacade<OutsourcingAgency>, IOutsourcingAgencyFacade
    {
        public OutsourcingAgencyFacade(IOutsourcingAgencyRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(OutsourcingAgency.Name);
    }
}
