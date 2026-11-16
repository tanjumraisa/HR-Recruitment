using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;


namespace HR.Facade.Implementation
{
    public class OfferLetterFacade : IOfferLetterFacade
    {
        private readonly IOfferLetterRepository _repo;
        private readonly IJobApplicationRepository _appRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IUserRepository _userRepo;
        

        public OfferLetterFacade(IOfferLetterRepository repo, IJobApplicationRepository appRepo, IEmployeeRepository employeeRepo, ICandidateRepository candidateRepo, IUserRepository userRepo)
        {
            _repo = repo;
            _appRepo = appRepo;
            _employeeRepo = employeeRepo;
            _candidateRepo = candidateRepo;
            _userRepo = userRepo;
        }

        public async Task<List<OfferLetter>> GetOfferLettersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<OfferLetter?> GetOfferLetterByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddOfferLetterAsync(OfferLetter entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.IssuedAt = DateTime.UtcNow;
            await _repo.AddAsync(e);

            var app = await _appRepo.GetByIdAsync(e.ApplicationId);
            if (app != null)
            {
                app.Status = ApplicationStatus.Offered;
                await _appRepo.UpdateAsync(app);
            }
        }

        public async Task<bool> UpdateOfferLetterAsync(OfferLetter entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.OfferId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.IssuedAt = existing.IssuedAt; e.ApplicationId = existing.ApplicationId;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteOfferLetterAsync(int id)
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

        private async Task ValidateAsync(OfferLetter e, OfferLetter? existing)
        {
            var app = await _appRepo.GetByIdAsync(e.ApplicationId) ?? throw new BusinessException("Application not found.");
            if (e.OfferedSalary <= 0) throw new BusinessException("Offered salary must be greater than zero.");
            if (existing == null)
            {
                if (app.Status != ApplicationStatus.Interview) throw new BusinessException("Offers can only be issued to candidates who have been interviewed.");
                if (await _repo.GetByApplicationIdAsync(e.ApplicationId) != null) throw new BusinessException("An offer already exists for this application.");
                if (e.JoiningDate.Date < DateTime.Today) throw new BusinessException("Joining date cannot be in the past.");
                e.Status = OfferStatus.Issued;
            }
        }

        // Accept  -> application Hired, employee profile created, user role changed to Employee.
        // Decline -> application Rejected.
        public async Task RespondAsync(int offerId, bool accept)
        {
            var offer = await _repo.GetByIdAsync(offerId) ?? throw new BusinessException("Offer not found.");
            if (offer.Status != OfferStatus.Issued) throw new BusinessException("This offer has already been answered.");

            var app = await _appRepo.GetByIdAsync(offer.ApplicationId) ?? throw new BusinessException("Application not found.");
            offer.Status = accept ? OfferStatus.Accepted : OfferStatus.Declined;
            await _repo.UpdateAsync(offer);

            app.Status = accept ? ApplicationStatus.Hired : ApplicationStatus.Rejected;
            await _appRepo.UpdateAsync(app);
            if (!accept) return;

            var candidate = await _candidateRepo.GetByIdAsync(app.CandidateId);
            if (candidate?.UserId == null) return; // candidate has no login account: HR creates the employee manually
            if (await _employeeRepo.GetByUserIdAsync(candidate.UserId.Value) != null) return;

            var job = app.JobOpening ?? throw new BusinessException("Job opening not found.");
            await _employeeRepo.AddAsync(new Employee
            {
                EmployeeCode = (await _employeeRepo.GetNextNumberAsync()).ToEmployeeCode(),
                UserId = candidate.UserId.Value,
                DepartmentId = job.DepartmentId,
                DesignationId = job.DesignationId,
                JoinDate = offer.JoiningDate,
                Salary = offer.OfferedSalary,
                Status = EmployeeStatus.Active
            });

            var user = await _userRepo.GetByIdAsync(candidate.UserId.Value);
            if (user != null)
            {
                user.RoleId = RoleIds.Employee;
                await _userRepo.UpdateAsync(user);
            }
        }
    }
}
