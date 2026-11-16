using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class GrievanceFacade : CrudFacade<Grievance>, IGrievanceFacade
    {
        public GrievanceFacade(IGrievanceRepository repo) : base(repo)
        {
        }

    }
}
