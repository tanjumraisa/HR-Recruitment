using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class AttendanceFacade : IAttendanceFacade
    {
        private readonly IAttendanceRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;
        

        public AttendanceFacade(IAttendanceRepository repo, IEmployeeRepository employeeRepo)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
        }

        public async Task<List<Attendance>> GetAttendancesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAttendanceAsync(Attendance entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateAttendanceAsync(Attendance entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.AttendanceId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                throw new BusinessException("This record is used by other data and cannot be deleted.");
            }
        }

        private async Task ValidateAsync(Attendance e, Attendance? existing)
        {
            if (await _employeeRepo.GetByIdAsync(e.EmployeeId) == null) throw new BusinessException("Employee not found.");
            e.Date = e.Date.Date;
            if (e.Date > DateTime.Today) throw new BusinessException("Attendance cannot be recorded for a future date.");
            if (e.CheckIn.HasValue && e.CheckOut.HasValue && e.CheckOut <= e.CheckIn)
                throw new BusinessException("Check-out must be after check-in.");
            var same = await _repo.GetByEmployeeDateAsync(e.EmployeeId, e.Date);
            if (same != null && same.AttendanceId != e.AttendanceId) throw new BusinessException("Attendance for this employee and date already exists.");
        }

        public async Task CheckInAsync(int employeeId)
        {
            var now = DateTime.Now;
            await AddAttendanceAsync(new Attendance
            {
                EmployeeId = employeeId,
                Date = now.Date,
                CheckIn = now.TimeOfDay,
                Status = now.TimeOfDay > AttendanceRules.OfficeStart.Add(AttendanceRules.GracePeriod)
                    ? AttendanceStatus.Late : AttendanceStatus.Present
            });
        }

        public async Task CheckOutAsync(int employeeId)
        {
            var now = DateTime.Now;
            var today = await _repo.GetByEmployeeDateAsync(employeeId, now.Date)
                ?? throw new BusinessException("You have not checked in today.");
            if (today.CheckOut.HasValue) throw new BusinessException("You have already checked out today.");

            today.CheckOut = now.TimeOfDay;
            await UpdateAttendanceAsync(today);
        }
    }
}
