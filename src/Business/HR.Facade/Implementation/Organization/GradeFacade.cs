using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class GradeFacade : CrudFacade<Grade>, IGradeFacade
    {
        public GradeFacade(IGradeRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Grade.Name);
    }
}
