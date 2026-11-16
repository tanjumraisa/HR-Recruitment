using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class LabourCaseFacade : CrudFacade<LabourCase>, ILabourCaseFacade
    {
        public LabourCaseFacade(ILabourCaseRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(LabourCase.CaseNo);
    }
}
