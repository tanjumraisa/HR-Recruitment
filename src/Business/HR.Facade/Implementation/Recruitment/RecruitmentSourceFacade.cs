using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class RecruitmentSourceFacade : CrudFacade<RecruitmentSource>, IRecruitmentSourceFacade
    {
        public RecruitmentSourceFacade(IRecruitmentSourceRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(RecruitmentSource.Name);
    }
}
