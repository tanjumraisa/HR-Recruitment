using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class DisciplinaryActionFacade : CrudFacade<DisciplinaryAction>, IDisciplinaryActionFacade
    {
        public DisciplinaryActionFacade(IDisciplinaryActionRepository repo) : base(repo)
        {
        }

    }
}
