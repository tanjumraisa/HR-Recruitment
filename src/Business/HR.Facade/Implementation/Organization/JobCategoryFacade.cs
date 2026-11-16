using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class JobCategoryFacade : CrudFacade<JobCategory>, IJobCategoryFacade
    {
        public JobCategoryFacade(IJobCategoryRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(JobCategory.Name);
    }
}
