using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

namespace HR.Facade.Implementation
{
    public class OutsourcedAttendanceFacade : CrudFacade<OutsourcedAttendance>, IOutsourcedAttendanceFacade
    {
        public OutsourcedAttendanceFacade(IOutsourcedAttendanceRepository repo) : base(repo)
        {
        }
        protected override string[] UniqueKeys => new[] { nameof(OutsourcedAttendance.OutsourcedEmployeeId), nameof(OutsourcedAttendance.Date) };
    }
}
