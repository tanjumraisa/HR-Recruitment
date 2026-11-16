using HR.Entities;
using HR.Facade.Implementation;
using HR.Framework;
using HR.Integration.Reporting;

// Business rule checks of the Facade layer (no database needed).
// Run:  dotnet run --project Tests/HR.Facade.Tests
var failed = 0;
async Task Check(string name, Func<Task<bool>> test)
{
    bool ok;
    try { ok = await test(); } catch (Exception ex) { ok = false; name += " (" + ex.GetType().Name + ": " + ex.Message + ")"; }
    Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
    if (!ok) failed++;
}
async Task<bool> Rejected(Func<Task> work)
{
    try { await work(); return false; } catch (BusinessException) { return true; }
}
var today = DateTime.Today;

// ---- Generic rules (CrudFacade) ----
var branches = new BranchFacade(new FakeBranchRepo());
await branches.AddAsync(new Branch { Name = "Head Office" });
await Check("Insert + get all active", async () => (await branches.GetAllAsync()).Count == 1);
await Check("Duplicate name is rejected (case-insensitive)", () => Rejected(() => branches.AddAsync(new Branch { Name = " head office " })));
await Check("Required field is checked", () => Rejected(() => branches.AddAsync(new Branch { Name = "" })));
await Check("Get by id", async () => (await branches.GetByIdAsync(1))?.Name == "Head Office");
await Check("Update", async () => { var b = (await branches.GetByIdAsync(1))!; b.Code = "HO"; return await branches.UpdateAsync(b) && (await branches.GetByIdAsync(1))!.Code == "HO"; });
await Check("Soft delete hides the row", async () => await branches.DeleteAsync(1) && (await branches.GetAllAsync()).Count == 0);
await Check("Name can be reused after delete", async () => { await branches.AddAsync(new Branch { Name = "Head Office" }); return true; });

var shifts = new EmployeeShiftFacade(new FakeEmployeeShiftRepo());
await Check("To-date before from-date is rejected", () => Rejected(() =>
    shifts.AddAsync(new EmployeeShift { EmployeeId = 1, ShiftId = 1, FromDate = today, ToDate = today.AddDays(-1) })));
await Check("Missing foreign key is rejected", () => Rejected(() => shifts.AddAsync(new EmployeeShift { EmployeeId = 0, ShiftId = 1 })));

var skills = new EmployeeSkillFacade(new FakeEmployeeSkillRepo());
await skills.AddAsync(new EmployeeSkill { EmployeeId = 1, SkillId = 2 });
await Check("Same employee + skill twice is rejected", () => Rejected(() => skills.AddAsync(new EmployeeSkill { EmployeeId = 1, SkillId = 2 })));
await Check("Invalid choice value is rejected", () => Rejected(() => skills.AddAsync(new EmployeeSkill { EmployeeId = 1, SkillId = 3, Proficiency = "Guru" })));

// ---- Loans ----
var loanRepo = new FakeLoanAdvanceRepo();
var loans = new LoanAdvanceFacade(loanRepo);
await loans.AddAsync(new LoanAdvance { ReferenceNo = "L-1", EmployeeId = 1, Amount = 12000, Installments = 12, PaidAmount = 999 });
await Check("Installment is calculated and paid amount starts at 0", async () =>
{ var l = (await loans.GetByIdAsync(1))!; return l.InstallmentAmount == 1000 && l.PaidAmount == 0 && l.Status == "Running"; });
var repayments = new LoanRepaymentFacade(new FakeLoanRepaymentRepo(), loanRepo);
await Check("Repayment above outstanding is rejected", () => Rejected(() => repayments.AddAsync(new LoanRepayment { LoanAdvanceId = 1, Amount = 13000 })));
await repayments.AddAsync(new LoanRepayment { LoanAdvanceId = 1, Amount = 2000 });
await Check("Repayment increases paid amount", async () => (await loans.GetByIdAsync(1))!.PaidAmount == 2000);
await repayments.AddAsync(new LoanRepayment { LoanAdvanceId = 1, Amount = 10000 });
await Check("Loan closes when fully paid", async () => (await loans.GetByIdAsync(1))!.Status == "Closed");
await repayments.DeleteAsync(2);
await Check("Deleting a repayment re-opens the loan", async () => { var l = (await loans.GetByIdAsync(1))!; return l.PaidAmount == 2000 && l.Status == "Running"; });

// ---- Employee profile ----
var dependents = new DependentFacade(new FakeDependentRepo());
await dependents.AddAsync(new Dependent { EmployeeId = 1, Name = "A", IsNominee = true, NomineeSharePercent = 70 });
await Check("Nominee shares above 100% are rejected", () => Rejected(() =>
    dependents.AddAsync(new Dependent { EmployeeId = 1, Name = "B", IsNominee = true, NomineeSharePercent = 40 })));

var banks = new BankAccountFacade(new FakeBankAccountRepo());
await banks.AddAsync(new BankAccount { EmployeeId = 1, BankName = "DBBL", AccountNumber = "1", IsPrimary = true });
await Check("Second salary account is rejected", () => Rejected(() =>
    banks.AddAsync(new BankAccount { EmployeeId = 1, BankName = "BRAC", AccountNumber = "2", IsPrimary = true })));

var grades = new FakeGradeRepo();
await grades.AddAsync(new Grade { Name = "Grade 2", MinSalary = 25000, MaxSalary = 40000 });
var contracts = new EmployeeContractFacade(new FakeContractRepo(), grades);
await Check("Basic salary outside the grade band is rejected", () => Rejected(() =>
    contracts.AddAsync(new EmployeeContract { EmployeeId = 1, EmploymentTypeId = 1, GradeId = 1, ContractNo = "C1", BasicSalary = 90000 })));
await contracts.AddAsync(new EmployeeContract { EmployeeId = 1, EmploymentTypeId = 1, GradeId = 1, ContractNo = "C1", BasicSalary = 30000 });
await Check("Second active contract is rejected", () => Rejected(() =>
    contracts.AddAsync(new EmployeeContract { EmployeeId = 1, EmploymentTypeId = 1, GradeId = 1, ContractNo = "C2", BasicSalary = 30000 })));

// ---- Training ----
var notes = new FakeNotifications();
var programs = new FakeTrainingProgramRepo();
await programs.AddAsync(new TrainingProgram { Title = "ASP.NET", Capacity = 1, StartDate = today, EndDate = today });
var enrol = new TrainingEnrollmentFacade(new FakeTrainingEnrollmentRepo(), programs, notes);
await enrol.AddAsync(new TrainingEnrollment { TrainingProgramId = 1, EmployeeId = 1 });
await Check("Employee is notified about enrolment", () => Task.FromResult(notes.Sent.Any(n => n.EmployeeId == 1)));
await Check("Full training is rejected", () => Rejected(() => enrol.AddAsync(new TrainingEnrollment { TrainingProgramId = 1, EmployeeId = 2 })));

// ---- Requests ----
var wfh = new WorkFromHomeRequestFacade(new FakeWfhRepo(), notes);
await wfh.AddAsync(new WorkFromHomeRequest { EmployeeId = 5, FromDate = today, ToDate = today.AddDays(2), Reason = "x" });
await Check("Overlapping work from home request is rejected", () => Rejected(() =>
    wfh.AddAsync(new WorkFromHomeRequest { EmployeeId = 5, FromDate = today.AddDays(1), ToDate = today.AddDays(3), Reason = "y" })));
await Check("Approving a request notifies the employee", async () =>
{ var r = (await wfh.GetByIdAsync(1))!; r.Status = RequestStatus.Approved; await wfh.UpdateAsync(r); return notes.Sent.Any(n => n.EmployeeId == 5); });

var swaps = new ShiftSwapRequestFacade(new FakeShiftSwapRepo(), notes);
await Check("Swapping a shift with yourself is rejected", () => Rejected(() =>
    swaps.AddAsync(new ShiftSwapRequest { RequesterId = 1, SwapWithId = 1, ShiftId = 1, SwapDate = today })));

var balances = new LeaveBalanceFacade(new FakeLeaveBalanceRepo());
await Check("Taken more than entitled is rejected", () => Rejected(() =>
    balances.AddAsync(new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, Year = 2026, Entitled = 10, Taken = 12 })));

// ---- Phase 2: transfer, promotion, TA/DA, canteen, fund, outsourced ----
var employees = new FakeEmployeeRepo();
await employees.AddAsync(new Employee { EmployeeCode = "EMP00001", UserId = 1, DepartmentId = 1, DesignationId = 1, BranchId = 1, Salary = 30000 });
var transfers = new EmployeeTransferFacade(new FakeTransferRepo(), notes, employees);
await Check("Transfer to the same department is rejected", () => Rejected(() =>
    transfers.AddAsync(new EmployeeTransfer { EmployeeId = 1, ToDepartmentId = 1 })));
await transfers.AddAsync(new EmployeeTransfer { EmployeeId = 1, ToDepartmentId = 2, ToBranchId = 2 });
await Check("Approved transfer moves the employee", async () =>
{ var t = (await transfers.GetByIdAsync(1))!; t.Status = RequestStatus.Approved; await transfers.UpdateAsync(t);
  var e = (await employees.GetByIdAsync(1))!; return e.DepartmentId == 2 && e.BranchId == 2 && t.FromDepartmentId == 1; });

var promotions = new EmployeePromotionFacade(new FakePromotionRepo(), notes, employees);
await Check("Promotion with lower salary is rejected", () => Rejected(() =>
    promotions.AddAsync(new EmployeePromotion { EmployeeId = 1, ToDesignationId = 2, NewSalary = 20000 })));
await promotions.AddAsync(new EmployeePromotion { EmployeeId = 1, ToDesignationId = 2, NewSalary = 36000 });
await Check("Approved promotion changes designation and salary", async () =>
{ var p = (await promotions.GetByIdAsync(1))!; p.Status = RequestStatus.Approved; await promotions.UpdateAsync(p);
  var e = (await employees.GetByIdAsync(1))!; return e.DesignationId == 2 && e.Salary == 36000 && p.OldSalary == 30000; });

var travel = new TravelExpenseFacade(new FakeTravelRepo(), notes);
await travel.AddAsync(new TravelExpense { EmployeeId = 1, TravelDate = today, FromPlace = "Dhaka", ToPlace = "Khulna", Purpose = "Audit", TransportCost = 900, FoodCost = 300 });
await Check("TA/DA total is calculated", async () => (await travel.GetByIdAsync(1))!.TotalAmount == 1200);
await Check("Claim cannot be paid before approval", async () => { var t = (await travel.GetByIdAsync(1))!; t.Status = "Paid"; return await Rejected(() => travel.UpdateAsync(t)); });

var items = new FakeCanteenItemRepo();
await items.AddAsync(new CanteenItem { Name = "Lunch", UnitPrice = 120, SubsidyPercent = 50 });
var meals = new CanteenMealFacade(new FakeCanteenMealRepo(), items);
await meals.AddAsync(new CanteenMeal { EmployeeId = 1, CanteenItemId = 1, Quantity = 2 });
await Check("Canteen amount uses the subsidy", async () => (await meals.GetByIdAsync(1))!.Amount == 120);

var members = new FakeFundMemberRepo();
await members.AddAsync(new FundMembership { FundId = 1, EmployeeId = 1 });
var contributions = new FakeFundContributionRepo();
await contributions.AddAsync(new FundContribution { FundMembershipId = 1, Month = 1, Year = 2026, EmployeeAmount = 3000, EmployerAmount = 3000 });
var withdrawals = new FundWithdrawalFacade(new FakeFundWithdrawalRepo(), contributions, members);
await Check("Fund withdrawal above balance is rejected", () => Rejected(() =>
    withdrawals.AddAsync(new FundWithdrawal { FundMembershipId = 1, Amount = 7000, Reason = "x" })));
await withdrawals.AddAsync(new FundWithdrawal { FundMembershipId = 1, Amount = 5000, Reason = "x", Status = "Approved" });
await Check("Second withdrawal counts the first one", () => Rejected(() =>
    withdrawals.AddAsync(new FundWithdrawal { FundMembershipId = 1, Amount = 2000, Reason = "y" })));

var workers = new FakeOutsourcedWorkerRepo();
await workers.AddAsync(new OutsourcedEmployee { FullName = "Karim", OutsourcingAgencyId = 1, DepartmentId = 1, DailyRate = 700 });
var oAttendance = new FakeOutsourcedAttendanceRepo();
for (var d = 1; d <= 3; d++) await oAttendance.AddAsync(new OutsourcedAttendance { OutsourcedEmployeeId = 1, Date = new DateTime(2026, 11, d), Status = d == 3 ? "Absent" : "Present" });
var oSalary = new OutsourcedSalaryFacade(new FakeOutsourcedSalaryRepo(), oAttendance, workers);
await oSalary.AddAsync(new OutsourcedSalary { OutsourcedEmployeeId = 1, Month = 11, Year = 2026 });
await Check("Outsourced wage = present days x daily rate", async () => { var s = (await oSalary.GetByIdAsync(1))!; return s.WorkingDays == 2 && s.Amount == 1400; });

// ---- Integration ----
await Check("CSV writer quotes values", () =>
    Task.FromResult(System.Text.Encoding.UTF8.GetString(new CsvWriter().Write(new[] { "A" }, new[] { new[] { "x\"y" } })).Contains("\"x\"\"y\"")));

Console.WriteLine(failed == 0 ? "All facade checks passed." : $"{failed} check(s) FAILED.");
return failed == 0 ? 0 : 1;
