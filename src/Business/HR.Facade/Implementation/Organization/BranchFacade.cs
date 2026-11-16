using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class BranchFacade : CrudFacade<Branch>, IBranchFacade
    {
        public BranchFacade(IBranchRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Branch.Name);
    }
}
