using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;

namespace HR.Facade.Implementation
{
    // Phase 2 modules with workflow rules (transfer, promotion, TA/DA, leave withdrawal,
    // outsourced salary, fund withdrawal, canteen).

    // Approving a transfer moves the employee to the new department / workstation.
    public class EmployeeTransferFacade : RequestFacade<EmployeeTransfer>, IEmployeeTransferFacade
    {
        private readonly IEmployeeRepository _employees;

        public EmployeeTransferFacade(IEmployeeTransferRepository repo, INotificationFacade n, IEmployeeRepository employees) : base(repo, n)
        {
            _employees = employees;
        }

        protected override int EmployeeOf(EmployeeTransfer e) => e.EmployeeId;
        protected override string Subject => "Transfer";

        protected override async Task ValidateAsync(EmployeeTransfer e, EmployeeTransfer? existing)
        {
            var emp = await _employees.GetByIdAsync(e.EmployeeId) ?? throw new BusinessException("Employee not found.");
            if (existing == null)
            {
                e.FromDepartmentId = emp.DepartmentId;
                e.FromBranchId = emp.BranchId;
            }
            else
            {
                e.FromDepartmentId = existing.FromDepartmentId;
                e.FromBranchId = existing.FromBranchId;
                if (existing.Status == RequestStatus.Approved) throw new BusinessException("An approved transfer cannot be changed.");
            }
            if (e.ToDepartmentId == e.FromDepartmentId && (e.ToBranchId == null || e.ToBranchId == e.FromBranchId))
                throw new BusinessException("Choose a different department or workstation.");
        }

        protected override async Task AfterUpdateAsync(EmployeeTransfer e, EmployeeTransfer existing)
        {
            if (e.Status == RequestStatus.Approved && existing.Status != RequestStatus.Approved)
            {
                var emp = await _employees.GetByIdAsync(e.EmployeeId);
                if (emp != null)
                {
                    emp.DepartmentId = e.ToDepartmentId;
                    if (e.ToBranchId.HasValue) emp.BranchId = e.ToBranchId;
                    await _employees.UpdateAsync(emp);
                }
            }
            await base.AfterUpdateAsync(e, existing);
        }
    }

    // Approving a promotion / increment changes the designation and salary of the employee.
    public class EmployeePromotionFacade : RequestFacade<EmployeePromotion>, IEmployeePromotionFacade
    {
        private readonly IEmployeeRepository _employees;

        public EmployeePromotionFacade(IEmployeePromotionRepository repo, INotificationFacade n, IEmployeeRepository employees) : base(repo, n)
        {
            _employees = employees;
        }

        protected override int EmployeeOf(EmployeePromotion e) => e.EmployeeId;
        protected override string Subject => "Promotion / increment";

        protected override async Task ValidateAsync(EmployeePromotion e, EmployeePromotion? existing)
        {
            var emp = await _employees.GetByIdAsync(e.EmployeeId) ?? throw new BusinessException("Employee not found.");
            if (existing == null)
            {
                e.FromDesignationId = emp.DesignationId;
                e.OldSalary = emp.Salary;
            }
            else
            {
                e.FromDesignationId = existing.FromDesignationId;
                e.OldSalary = existing.OldSalary;
                if (existing.Status == RequestStatus.Approved) throw new BusinessException("An approved promotion cannot be changed.");
            }
            if (e.NewSalary <= 0) throw new BusinessException("New salary must be greater than zero.");
            if (e.PromotionType != "Demotion" && e.NewSalary < e.OldSalary)
                throw new BusinessException("For a promotion or increment the new salary cannot be lower than the old salary.");
        }

        protected override async Task AfterUpdateAsync(EmployeePromotion e, EmployeePromotion existing)
        {
            if (e.Status == RequestStatus.Approved && existing.Status != RequestStatus.Approved)
            {
                var emp = await _employees.GetByIdAsync(e.EmployeeId);
                if (emp != null)
                {
                    emp.DesignationId = e.ToDesignationId;
                    emp.Salary = e.NewSalary;
                    await _employees.UpdateAsync(emp);
                }
            }
            await base.AfterUpdateAsync(e, existing);
        }
    }

    // TA/DA claim: total is calculated, the employee cannot approve his own claim.
    public class TravelExpenseFacade : RequestFacade<TravelExpense>, ITravelExpenseFacade
    {
        public TravelExpenseFacade(ITravelExpenseRepository repo, INotificationFacade n) : base(repo, n) { }
        protected override int EmployeeOf(TravelExpense e) => e.EmployeeId;
        protected override string Subject => "TA/DA claim";

        protected override Task ValidateAsync(TravelExpense e, TravelExpense? existing)
        {
            e.TotalAmount = e.TransportCost + e.FoodCost + e.HotelCost + e.OtherCost;
            if (e.TotalAmount <= 0) throw new BusinessException("Enter at least one cost.");
            if (e.TravelDate.Date > DateTime.Today) throw new BusinessException("Travel date cannot be in the future.");
            if (e.ApprovedById.HasValue && e.ApprovedById == e.EmployeeId) throw new BusinessException("An employee cannot approve his own claim.");
            if (e.Status == "Paid" && existing?.Status != "Approved" && existing?.Status != "Paid")
                throw new BusinessException("Only an approved claim can be marked paid.");
            if (existing?.Status == "Paid" && e.Status != "Paid") throw new BusinessException("A paid claim cannot be changed.");
            return Task.CompletedTask;
        }
    }

    // Approving a withdrawal cancels the approved leave.
    public class LeaveWithdrawalFacade : RequestFacade<LeaveWithdrawal>, ILeaveWithdrawalFacade
    {
        private readonly ILeaveRequestRepository _leaves;

        public LeaveWithdrawalFacade(ILeaveWithdrawalRepository repo, INotificationFacade n, ILeaveRequestRepository leaves) : base(repo, n)
        {
            _leaves = leaves;
        }

        protected override int EmployeeOf(LeaveWithdrawal e) => e.EmployeeId;
        protected override string Subject => "Leave withdrawal";

        protected override async Task ValidateAsync(LeaveWithdrawal e, LeaveWithdrawal? existing)
        {
            var leave = await _leaves.GetByIdAsync(e.LeaveRequestId) ?? throw new BusinessException("Leave not found.");
            if (leave.EmployeeId != e.EmployeeId) throw new BusinessException("This leave belongs to another employee.");
            if (existing == null && leave.Status != LeaveStatus.Approved && leave.Status != LeaveStatus.Pending)
                throw new BusinessException("Only a pending or approved leave can be withdrawn.");
            if (existing?.Status == RequestStatus.Approved) throw new BusinessException("An approved withdrawal cannot be changed.");
        }

        protected override async Task AfterUpdateAsync(LeaveWithdrawal e, LeaveWithdrawal existing)
        {
            if (e.Status == RequestStatus.Approved && existing.Status != RequestStatus.Approved)
            {
                var leave = await _leaves.GetByIdAsync(e.LeaveRequestId);
                if (leave != null)
                {
                    leave.Status = LeaveStatus.Cancelled;
                    await _leaves.UpdateAsync(leave);
                }
            }
            await base.AfterUpdateAsync(e, existing);
        }
    }

    // Wage = present days in the month (from outsourced attendance) x daily rate.
    public class OutsourcedSalaryFacade : CrudFacade<OutsourcedSalary>, IOutsourcedSalaryFacade
    {
        private readonly IOutsourcedAttendanceRepository _attendance;
        private readonly IOutsourcedEmployeeRepository _workers;

        public OutsourcedSalaryFacade(IOutsourcedSalaryRepository repo, IOutsourcedAttendanceRepository attendance, IOutsourcedEmployeeRepository workers) : base(repo)
        {
            _attendance = attendance;
            _workers = workers;
        }

        protected override string[] UniqueKeys => new[] { nameof(OutsourcedSalary.OutsourcedEmployeeId), nameof(OutsourcedSalary.Month), nameof(OutsourcedSalary.Year) };

        protected override async Task ValidateAsync(OutsourcedSalary e, OutsourcedSalary? existing)
        {
            if (existing?.Status == "Paid") throw new BusinessException("A paid salary cannot be changed.");
            var worker = await _workers.GetByIdAsync(e.OutsourcedEmployeeId) ?? throw new BusinessException("Worker not found.");
            if (e.DailyRate <= 0) e.DailyRate = worker.DailyRate;
            e.WorkingDays = (await _attendance.GetAllAsync())
                .Count(a => a.OutsourcedEmployeeId == e.OutsourcedEmployeeId && a.Date.Month == e.Month && a.Date.Year == e.Year && a.Status == "Present");
            e.Amount = e.WorkingDays * e.DailyRate;
        }
    }

    // A member can withdraw at most his balance (contributions - earlier withdrawals).
    public class FundWithdrawalFacade : CrudFacade<FundWithdrawal>, IFundWithdrawalFacade
    {
        private readonly IFundContributionRepository _contributions;
        private readonly IFundMembershipRepository _members;

        public FundWithdrawalFacade(IFundWithdrawalRepository repo, IFundContributionRepository contributions, IFundMembershipRepository members) : base(repo)
        {
            _contributions = contributions;
            _members = members;
        }

        protected override async Task ValidateAsync(FundWithdrawal e, FundWithdrawal? existing)
        {
            if (e.Amount <= 0) throw new BusinessException("Amount must be greater than zero.");
            var member = await _members.GetByIdAsync(e.FundMembershipId) ?? throw new BusinessException("Fund member not found.");
            if (member.Status != "Active" && existing == null) throw new BusinessException("This fund membership is closed.");
            var paidIn = (await _contributions.GetAllAsync()).Where(c => c.FundMembershipId == e.FundMembershipId).Sum(c => c.EmployeeAmount + c.EmployerAmount);
            var taken = (await Repo.GetAllAsync())
                .Where(w => w.FundMembershipId == e.FundMembershipId && w.FundWithdrawalId != e.FundWithdrawalId && (w.Status == "Approved" || w.Status == "Paid"))
                .Sum(w => w.Amount);
            if (e.Amount > paidIn - taken) throw new BusinessException($"Amount is more than the fund balance ({paidIn - taken:N2}).");
        }
    }

    // Employee pays: quantity x unit price minus the company subsidy.
    public class CanteenMealFacade : CrudFacade<CanteenMeal>, ICanteenMealFacade
    {
        private readonly ICanteenItemRepository _items;

        public CanteenMealFacade(ICanteenMealRepository repo, ICanteenItemRepository items) : base(repo)
        {
            _items = items;
        }

        protected override async Task ValidateAsync(CanteenMeal e, CanteenMeal? existing)
        {
            var item = await _items.GetByIdAsync(e.CanteenItemId) ?? throw new BusinessException("Canteen item not found.");
            e.Amount = Math.Round(e.Quantity * item.UnitPrice * (1 - item.SubsidyPercent / 100m), 2);
        }
    }
}
