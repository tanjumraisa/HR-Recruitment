using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class EmployeeFacade : IEmployeeFacade
    {
        private readonly IEmployeeRepository _repo;
        private readonly IUserRepository _userRepo;
        

        public EmployeeFacade(IEmployeeRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddEmployeeAsync(Employee entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.EmployeeCode = (await _repo.GetNextNumberAsync()).ToEmployeeCode();
            await _repo.AddAsync(e);

            var user = await _userRepo.GetByIdAsync(e.UserId);
            if (user != null && user.RoleId == RoleIds.Candidate)
            {
                user.RoleId = RoleIds.Employee;
                await _userRepo.UpdateAsync(user);
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.EmployeeId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.EmployeeCode = existing.EmployeeCode;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
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

        private async Task ValidateAsync(Employee e, Employee? existing)
        {
            var user = await _userRepo.GetByIdAsync(e.UserId);
            if (user == null) throw new BusinessException("Selected user does not exist.");
            var byUser = await _repo.GetByUserIdAsync(e.UserId);
            if (byUser != null && byUser.EmployeeId != e.EmployeeId) throw new BusinessException("This user is already an employee.");
            if (e.Salary <= 0) throw new BusinessException("Salary must be greater than zero.");
            if (e.ManagerId.HasValue && e.ManagerId.Value == e.EmployeeId) throw new BusinessException("An employee cannot be their own manager.");
            if (!EmployeeStatus.All.Contains(e.Status)) throw new BusinessException("Invalid employee status.");
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }
    }
}
