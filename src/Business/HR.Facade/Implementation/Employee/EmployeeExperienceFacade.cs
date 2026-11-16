using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmployeeExperienceFacade : CrudFacade<EmployeeExperience>, IEmployeeExperienceFacade
    {
        public EmployeeExperienceFacade(IEmployeeExperienceRepository repo) : base(repo)
        {
        }

    }
}
