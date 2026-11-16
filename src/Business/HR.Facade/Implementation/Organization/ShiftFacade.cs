using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class ShiftFacade : CrudFacade<Shift>, IShiftFacade
    {
        public ShiftFacade(IShiftRepository repo) : base(repo)
        {
        }
        protected override string? UniqueProperty => nameof(Shift.Name);
    }
}
