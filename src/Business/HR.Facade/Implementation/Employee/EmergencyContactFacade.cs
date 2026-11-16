using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class EmergencyContactFacade : CrudFacade<EmergencyContact>, IEmergencyContactFacade
    {
        public EmergencyContactFacade(IEmergencyContactRepository repo) : base(repo)
        {
        }

    }
}
