using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;

namespace HR.Facade.Implementation
{
    // HRMS modules that have extra business rules. The other modules use CrudFacade as it is.

    // Base for approval requests: the employee gets a notification when the status changes.
    public abstract class RequestFacade<TEntity> : CrudFacade<TEntity> where TEntity : class
    {
        protected readonly INotificationFacade Notifications;

        protected RequestFacade(IRepository<TEntity> repo, INotificationFacade notifications) : base(repo)
        {
            Notifications = notifications;
        }

        protected abstract int EmployeeOf(TEntity entity);
        protected abstract string Subject { get; }
        protected virtual string Link(TEntity entity) => $"/records/{typeof(TEntity).Name}/{KeyOf(entity)}";

        protected override async Task AfterUpdateAsync(TEntity entity, TEntity existing)
        {
            var status = StatusOf(entity);
            if (status != null && status != StatusOf(existing))
                await Notifications.NotifyEmployeeAsync(EmployeeOf(entity), $"{Subject}: {status}",
                    $"Your {Subject.ToLower()} #{KeyOf(entity)} is now {status}.", Link(entity));
        }
    }

    public class WorkFromHomeRequestFacade : RequestFacade<WorkFromHomeRequest>, IWorkFromHomeRequestFacade
    {
        public WorkFromHomeRequestFacade(IWorkFromHomeRequestRepository repo, INotificationFacade n) : base(repo, n) { }
        protected override int EmployeeOf(WorkFromHomeRequest e) => e.EmployeeId;
        protected override string Subject => "Work from home request";

        protected override async Task ValidateAsync(WorkFromHomeRequest e, WorkFromHomeRequest? existing)
        {
            if ((e.ToDate - e.FromDate).TotalDays > 30) throw new BusinessException("A work from home request can be at most 31 days.");
            if (e.ApprovedById.HasValue && e.ApprovedById == e.EmployeeId) throw new BusinessException("An employee cannot approve his own request.");
            if (e.Status == RequestStatus.Pending) e.ApprovedById = null;
            var all = await Repo.GetAllAsync();
            if (all.Any(x => x.WorkFromHomeRequestId != e.WorkFromHomeRequestId && x.EmployeeId == e.EmployeeId && x.Status != RequestStatus.Rejected
                             && x.FromDate <= e.ToDate && e.FromDate <= x.ToDate))
                throw new BusinessException("This employee already has a work from home request for these dates.");
        }
    }

    public class ShiftSwapRequestFacade : RequestFacade<ShiftSwapRequest>, IShiftSwapRequestFacade
    {
        public ShiftSwapRequestFacade(IShiftSwapRequestRepository repo, INotificationFacade n) : base(repo, n) { }
        protected override int EmployeeOf(ShiftSwapRequest e) => e.RequesterId;
        protected override string Subject => "Shift swap request";

        protected override Task ValidateAsync(ShiftSwapRequest e, ShiftSwapRequest? existing)
        {
            if (e.RequesterId == e.SwapWithId) throw new BusinessException("An employee cannot swap a shift with himself.");
            if (existing == null && e.SwapDate.Date < DateTime.Today) throw new BusinessException("Swap date cannot be in the past.");
            return Task.CompletedTask;
        }

        protected override async Task AfterUpdateAsync(ShiftSwapRequest entity, ShiftSwapRequest existing)
        {
            await base.AfterUpdateAsync(entity, existing);
            if (entity.Status != existing.Status)
                await Notifications.NotifyEmployeeAsync(entity.SwapWithId, $"Shift swap: {entity.Status}",
                    $"Shift swap on {entity.SwapDate:dd MMM yyyy} is {entity.Status}.", Link(entity));
        }
    }

    public class OvertimeRecordFacade : RequestFacade<OvertimeRecord>, IOvertimeRecordFacade
    {
        public OvertimeRecordFacade(IOvertimeRecordRepository repo, INotificationFacade n) : base(repo, n) { }
        protected override int EmployeeOf(OvertimeRecord e) => e.EmployeeId;
        protected override string Subject => "Overtime";

        protected override Task ValidateAsync(OvertimeRecord e, OvertimeRecord? existing)
        {
            if (e.Hours <= 0) throw new BusinessException("Overtime hours must be greater than zero.");
            if (e.Date.Date > DateTime.Today) throw new BusinessException("Overtime cannot be recorded for a future date.");
            if (existing?.Status == "Paid" && e.Status != "Paid") throw new BusinessException("Paid overtime cannot be changed.");
            return Task.CompletedTask;
        }
    }

    // Approving a correction writes the requested punch times into the attendance record.
    public class AttendanceCorrectionFacade : RequestFacade<AttendanceCorrection>, IAttendanceCorrectionFacade
    {
        private readonly IAttendanceRepository _attendance;

        public AttendanceCorrectionFacade(IAttendanceCorrectionRepository repo, INotificationFacade n, IAttendanceRepository attendance) : base(repo, n)
        {
            _attendance = attendance;
        }

        protected override int EmployeeOf(AttendanceCorrection e) => e.EmployeeId;
        protected override string Subject => "Attendance correction";

        protected override async Task ValidateAsync(AttendanceCorrection e, AttendanceCorrection? existing)
        {
            var att = await _attendance.GetByIdAsync(e.AttendanceId) ?? throw new BusinessException("Attendance record not found.");
            if (att.EmployeeId != e.EmployeeId) throw new BusinessException("The attendance record belongs to another employee.");
            if (e.RequestedCheckIn == null && e.RequestedCheckOut == null) throw new BusinessException("Give the corrected check-in or check-out time.");
            if (e.RequestedCheckIn.HasValue && e.RequestedCheckOut.HasValue && e.RequestedCheckOut < e.RequestedCheckIn)
                throw new BusinessException("Check-out cannot be before check-in.");
            if (existing?.Status == RequestStatus.Approved && e.Status != RequestStatus.Approved)
                throw new BusinessException("An approved correction cannot be changed.");
        }

        protected override async Task AfterUpdateAsync(AttendanceCorrection e, AttendanceCorrection existing)
        {
            if (e.Status == RequestStatus.Approved && existing.Status != RequestStatus.Approved)
            {
                var att = await _attendance.GetByIdAsync(e.AttendanceId);
                if (att != null)
                {
                    if (e.RequestedCheckIn.HasValue) att.CheckIn = e.RequestedCheckIn;
                    if (e.RequestedCheckOut.HasValue) att.CheckOut = e.RequestedCheckOut;
                    if (att.CheckIn.HasValue)
                        att.Status = att.CheckIn.Value > AttendanceRules.OfficeStart + AttendanceRules.GracePeriod ? AttendanceStatus.Late : AttendanceStatus.Present;
                    await _attendance.UpdateAsync(att);
                }
            }
            await base.AfterUpdateAsync(e, existing);
        }
    }

    public class ManpowerRequisitionFacade : RequestFacade<ManpowerRequisition>, IManpowerRequisitionFacade
    {
        public ManpowerRequisitionFacade(IManpowerRequisitionRepository repo, INotificationFacade n) : base(repo, n) { }
        protected override int EmployeeOf(ManpowerRequisition e) => e.RequestedById;
        protected override string Subject => "Manpower requisition";

        protected override Task ValidateAsync(ManpowerRequisition e, ManpowerRequisition? existing)
        {
            if (existing == null && e.RequiredBy.Date < DateTime.Today) throw new BusinessException("Required-by date cannot be in the past.");
            if (e.Status == "Fulfilled" && e.JobOpeningId == null) throw new BusinessException("Select the job opening before marking the requisition fulfilled.");
            return Task.CompletedTask;
        }
    }

    // Capacity of the training is checked; the employee is told about the enrolment.
    public class TrainingEnrollmentFacade : CrudFacade<TrainingEnrollment>, ITrainingEnrollmentFacade
    {
        private readonly ITrainingProgramRepository _programs;
        private readonly INotificationFacade _notifications;

        public TrainingEnrollmentFacade(ITrainingEnrollmentRepository repo, ITrainingProgramRepository programs, INotificationFacade notifications) : base(repo)
        {
            _programs = programs;
            _notifications = notifications;
        }

        protected override string[] UniqueKeys => new[] { nameof(TrainingEnrollment.TrainingProgramId), nameof(TrainingEnrollment.EmployeeId) };

        protected override async Task ValidateAsync(TrainingEnrollment e, TrainingEnrollment? existing)
        {
            var program = await _programs.GetByIdAsync(e.TrainingProgramId) ?? throw new BusinessException("Training not found.");
            if (program.Status == "Cancelled") throw new BusinessException("This training is cancelled.");
            if (e.Status != "Cancelled")
            {
                var taken = (await Repo.GetAllAsync()).Count(x => x.TrainingProgramId == e.TrainingProgramId && x.Status != "Cancelled" && x.TrainingEnrollmentId != e.TrainingEnrollmentId);
                if (taken >= program.Capacity) throw new BusinessException($"The training is full ({program.Capacity} seats).");
            }
            if (e.Score.HasValue && e.Status != "Completed") throw new BusinessException("A score can be given only when the training is completed.");
        }

        protected override async Task AfterAddAsync(TrainingEnrollment e)
        {
            var program = await _programs.GetByIdAsync(e.TrainingProgramId);
            await _notifications.NotifyEmployeeAsync(e.EmployeeId, "Training enrolment",
                $"You are enrolled in '{program?.Title}' starting {program?.StartDate:dd MMM yyyy}.", $"/records/TrainingProgram/{e.TrainingProgramId}");
        }
    }

    public class LoanAdvanceFacade : CrudFacade<LoanAdvance>, ILoanAdvanceFacade
    {
        public LoanAdvanceFacade(ILoanAdvanceRepository repo) : base(repo) { }

        protected override string? UniqueProperty => nameof(LoanAdvance.ReferenceNo);

        protected override Task ValidateAsync(LoanAdvance e, LoanAdvance? existing)
        {
            if (e.Amount <= 0) throw new BusinessException("Loan amount must be greater than zero.");
            if (e.InstallmentAmount <= 0) e.InstallmentAmount = Math.Round(e.Amount / Math.Max(1, e.Installments), 2);
            if (existing != null) e.PaidAmount = existing.PaidAmount;   // changed only by repayments
            else e.PaidAmount = 0;
            if (e.PaidAmount > e.Amount) throw new BusinessException("Amount cannot be less than the amount already paid.");
            e.Status = e.PaidAmount >= e.Amount ? "Closed" : "Running";
            return Task.CompletedTask;
        }
    }

    // Each repayment reduces the outstanding loan and closes it when fully paid.
    public class LoanRepaymentFacade : CrudFacade<LoanRepayment>, ILoanRepaymentFacade
    {
        private readonly ILoanAdvanceRepository _loans;

        public LoanRepaymentFacade(ILoanRepaymentRepository repo, ILoanAdvanceRepository loans) : base(repo)
        {
            _loans = loans;
        }

        protected override async Task ValidateAsync(LoanRepayment e, LoanRepayment? existing)
        {
            if (e.Amount <= 0) throw new BusinessException("Amount must be greater than zero.");
            var loan = await _loans.GetByIdAsync(e.LoanAdvanceId) ?? throw new BusinessException("Loan not found.");
            if (existing != null && existing.LoanAdvanceId != e.LoanAdvanceId) throw new BusinessException("The loan of a repayment cannot be changed.");
            var outstanding = loan.Amount - loan.PaidAmount + (existing?.Amount ?? 0);
            if (e.Amount > outstanding) throw new BusinessException($"Amount is more than the outstanding balance ({outstanding:N2}).");
        }

        protected override Task AfterAddAsync(LoanRepayment e) => ApplyAsync(e.LoanAdvanceId, e.Amount);
        protected override Task AfterUpdateAsync(LoanRepayment e, LoanRepayment existing) => ApplyAsync(e.LoanAdvanceId, e.Amount - existing.Amount);
        protected override Task BeforeDeleteAsync(LoanRepayment existing) => ApplyAsync(existing.LoanAdvanceId, -existing.Amount);

        private async Task ApplyAsync(int loanId, decimal change)
        {
            var loan = await _loans.GetByIdAsync(loanId);
            if (loan == null || change == 0) return;
            loan.PaidAmount = Math.Max(0, loan.PaidAmount + change);
            loan.Status = loan.PaidAmount >= loan.Amount ? "Closed" : "Running";
            await _loans.UpdateAsync(loan);
        }
    }

    // Nominee shares of one employee may not pass 100 %.
    public class DependentFacade : CrudFacade<Dependent>, IDependentFacade
    {
        public DependentFacade(IDependentRepository repo) : base(repo) { }

        protected override async Task ValidateAsync(Dependent e, Dependent? existing)
        {
            if (!e.IsNominee) e.NomineeSharePercent = 0;
            if (e.DateOfBirth.HasValue && e.DateOfBirth.Value.Date > DateTime.Today) throw new BusinessException("Date of birth cannot be in the future.");
            var others = (await Repo.GetAllAsync()).Where(x => x.EmployeeId == e.EmployeeId && x.IsNominee && x.DependentId != e.DependentId).Sum(x => x.NomineeSharePercent);
            if (others + e.NomineeSharePercent > 100) throw new BusinessException($"Total nominee share would be {others + e.NomineeSharePercent}% (maximum 100%).");
        }
    }

    // Only one salary account per employee.
    public class BankAccountFacade : CrudFacade<BankAccount>, IBankAccountFacade
    {
        public BankAccountFacade(IBankAccountRepository repo) : base(repo) { }

        protected override async Task ValidateAsync(BankAccount e, BankAccount? existing)
        {
            var all = await Repo.GetAllAsync();
            if (e.IsPrimary && all.Any(x => x.EmployeeId == e.EmployeeId && x.IsPrimary && x.BankAccountId != e.BankAccountId))
                throw new BusinessException("This employee already has a salary account. Untick it there first.");
            if (all.Any(x => x.BankAccountId != e.BankAccountId && x.BankName == e.BankName && x.AccountNumber == e.AccountNumber))
                throw new BusinessException("This account number is already saved.");
        }
    }

    // Only one active contract per employee; the basic salary must fit the grade.
    public class EmployeeContractFacade : CrudFacade<EmployeeContract>, IEmployeeContractFacade
    {
        private readonly IGradeRepository _grades;

        public EmployeeContractFacade(IEmployeeContractRepository repo, IGradeRepository grades) : base(repo)
        {
            _grades = grades;
        }

        protected override string? UniqueProperty => nameof(EmployeeContract.ContractNo);

        protected override async Task ValidateAsync(EmployeeContract e, EmployeeContract? existing)
        {
            var grade = await _grades.GetByIdAsync(e.GradeId) ?? throw new BusinessException("Grade not found.");
            if (grade.MaxSalary > 0 && (e.BasicSalary < grade.MinSalary || e.BasicSalary > grade.MaxSalary))
                throw new BusinessException($"Basic salary must be between {grade.MinSalary:N0} and {grade.MaxSalary:N0} for {grade.Name}.");
            if (e.Status == "Active" && (await Repo.GetAllAsync()).Any(x => x.EmployeeId == e.EmployeeId && x.Status == "Active" && x.EmployeeContractId != e.EmployeeContractId))
                throw new BusinessException("This employee already has an active contract. Expire it first.");
        }
    }

    public class LeaveBalanceFacade : CrudFacade<LeaveBalance>, ILeaveBalanceFacade
    {
        public LeaveBalanceFacade(ILeaveBalanceRepository repo) : base(repo) { }

        protected override string[] UniqueKeys => new[] { nameof(LeaveBalance.EmployeeId), nameof(LeaveBalance.LeaveTypeId), nameof(LeaveBalance.Year) };

        protected override Task ValidateAsync(LeaveBalance e, LeaveBalance? existing)
        {
            if (e.Taken > e.Entitled + e.CarriedForward) throw new BusinessException("Taken days cannot be more than entitled + carried forward days.");
            return Task.CompletedTask;
        }
    }
}
