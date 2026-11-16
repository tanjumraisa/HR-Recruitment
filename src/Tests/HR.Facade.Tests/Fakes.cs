using System.Reflection;
using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;

// In-memory repositories, so the business rules can be tested without SQL Server.
public class FakeRepository<T> : IRepository<T> where T : class
{
    private static readonly PropertyInfo Key = typeof(T).GetProperty(typeof(T).Name + "Id")!;
    private static readonly MethodInfo Clone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;
    public readonly List<T> Rows = new();
    private int _next = 1;

    private static T Copy(T e) => (T)Clone.Invoke(e, null)!;
    private static bool IsDeleted(T e) => e is ISoftDelete s && s.IsDeleted;

    public Task<List<T>> GetAllAsync() => Task.FromResult(Rows.Where(r => !IsDeleted(r)).Select(Copy).ToList());
    public Task<T?> GetByIdAsync(int id) => Task.FromResult(Rows.Where(r => !IsDeleted(r) && (int)Key.GetValue(r)! == id).Select(Copy).FirstOrDefault());
    public Task AddAsync(T entity) { Key.SetValue(entity, _next++); Rows.Add(Copy(entity)); return Task.CompletedTask; }
    public Task UpdateAsync(T entity)
    {
        var i = Rows.FindIndex(r => (int)Key.GetValue(r)! == (int)Key.GetValue(entity)!);
        Rows[i] = Copy(entity);
        return Task.CompletedTask;
    }
    public Task<bool> DeleteAsync(int id)
    {
        var row = Rows.FirstOrDefault(r => (int)Key.GetValue(r)! == id);
        if (row == null) return Task.FromResult(false);
        if (row is ISoftDelete s) { s.IsDeleted = true; s.DeletedAt = DateTime.UtcNow; } else Rows.Remove(row);
        return Task.FromResult(true);
    }
}

public class FakeBranchRepo : FakeRepository<Branch>, IBranchRepository { }
public class FakeEmployeeShiftRepo : FakeRepository<EmployeeShift>, IEmployeeShiftRepository { }
public class FakeEmployeeSkillRepo : FakeRepository<EmployeeSkill>, IEmployeeSkillRepository { }
public class FakeLoanAdvanceRepo : FakeRepository<LoanAdvance>, ILoanAdvanceRepository { }
public class FakeLoanRepaymentRepo : FakeRepository<LoanRepayment>, ILoanRepaymentRepository { }
public class FakeDependentRepo : FakeRepository<Dependent>, IDependentRepository { }
public class FakeBankAccountRepo : FakeRepository<BankAccount>, IBankAccountRepository { }
public class FakeContractRepo : FakeRepository<EmployeeContract>, IEmployeeContractRepository { }
public class FakeGradeRepo : FakeRepository<Grade>, IGradeRepository { }
public class FakeTrainingProgramRepo : FakeRepository<TrainingProgram>, ITrainingProgramRepository { }
public class FakeTrainingEnrollmentRepo : FakeRepository<TrainingEnrollment>, ITrainingEnrollmentRepository { }
public class FakeWfhRepo : FakeRepository<WorkFromHomeRequest>, IWorkFromHomeRequestRepository { }
public class FakeShiftSwapRepo : FakeRepository<ShiftSwapRequest>, IShiftSwapRequestRepository { }
public class FakeLeaveBalanceRepo : FakeRepository<LeaveBalance>, ILeaveBalanceRepository { }

public class FakeNotifications : INotificationFacade
{
    public readonly List<(int EmployeeId, string Title)> Sent = new();
    public Task<List<Notification>> GetMineAsync(int userId, int take = 50) => Task.FromResult(new List<Notification>());
    public Task<int> CountUnreadAsync(int userId) => Task.FromResult(0);
    public Task<Notification?> OpenAsync(int id, int userId) => Task.FromResult<Notification?>(null);
    public Task MarkAllReadAsync(int userId) => Task.CompletedTask;
    public Task NotifyUserAsync(int userId, string title, string message, string? link = null) => Task.CompletedTask;
    public Task NotifyEmployeeAsync(int employeeId, string title, string message, string? link = null)
    {
        Sent.Add((employeeId, title));
        return Task.CompletedTask;
    }
}

public class FakeEmployeeRepo : FakeRepository<Employee>, IEmployeeRepository
{
    public Task<Employee?> GetByUserIdAsync(int userId) => Task.FromResult<Employee?>(null);
    public Task<List<Employee>> GetActiveAsync() => GetAllAsync();
    public Task<int> GetNextNumberAsync() => Task.FromResult(Rows.Count + 1);
}
public class FakeTransferRepo : FakeRepository<EmployeeTransfer>, IEmployeeTransferRepository { }
public class FakePromotionRepo : FakeRepository<EmployeePromotion>, IEmployeePromotionRepository { }
public class FakeTravelRepo : FakeRepository<TravelExpense>, ITravelExpenseRepository { }
public class FakeCanteenItemRepo : FakeRepository<CanteenItem>, ICanteenItemRepository { }
public class FakeCanteenMealRepo : FakeRepository<CanteenMeal>, ICanteenMealRepository { }
public class FakeFundMemberRepo : FakeRepository<FundMembership>, IFundMembershipRepository { }
public class FakeFundContributionRepo : FakeRepository<FundContribution>, IFundContributionRepository { }
public class FakeFundWithdrawalRepo : FakeRepository<FundWithdrawal>, IFundWithdrawalRepository { }
public class FakeOutsourcedWorkerRepo : FakeRepository<OutsourcedEmployee>, IOutsourcedEmployeeRepository { }
public class FakeOutsourcedAttendanceRepo : FakeRepository<OutsourcedAttendance>, IOutsourcedAttendanceRepository { }
public class FakeOutsourcedSalaryRepo : FakeRepository<OutsourcedSalary>, IOutsourcedSalaryRepository { }
