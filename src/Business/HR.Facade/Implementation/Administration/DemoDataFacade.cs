using HR.DataAccess;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HR.Facade.Implementation
{
    // Fills the database with realistic sample data for testing and for the project demo.
    // Runs once (demo users have e-mails ending in @demo.hr). Every demo user's password: Demo@12345
    public class DemoDataFacade : IDemoDataFacade
    {
        public const string DemoDomain = "@demo.hr";
        public const string DemoPassword = "Demo@12345";
        private readonly ApplicationDbContext _db;
        private readonly Random _rnd = new(42);

        public DemoDataFacade(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> IsLoadedAsync() => _db.Users.AnyAsync(u => u.Email.EndsWith(DemoDomain));

        public async Task<string> LoadAsync()
        {
            if (await IsLoadedAsync()) throw new BusinessException("Demo data is already loaded.");
            var admin = await _db.Users.FirstOrDefaultAsync(u => u.RoleId == RoleIds.Admin) ?? throw new BusinessException("Admin user not found.");

            _db.SkipAudit = true;
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var today = DateTime.Today;
                var hasher = new PasswordHasher<User>();

                // ---------- Departments and designations ----------
                var deptNames = new[] { "Information Technology", "Human Resources", "Accounts", "Sales and Marketing", "Administration" };
                var depts = new List<Department>();
                foreach (var n in deptNames)
                    depts.Add(await _db.Departments.FirstOrDefaultAsync(d => d.Name == n) ?? Add(new Department { Name = n, Description = n + " department" }));
                var desigData = new (string Title, decimal Salary)[]
                {
                    ("Manager", 90000), ("Senior Software Engineer", 70000), ("Software Engineer", 45000), ("HR Executive", 35000),
                    ("Accountant", 38000), ("Sales Executive", 30000), ("Office Assistant", 18000)
                };
                var desigs = new List<Designation>();
                foreach (var (t, sal) in desigData)
                    desigs.Add(await _db.Designations.FirstOrDefaultAsync(d => d.Title == t) ?? Add(new Designation { Title = t, BaseSalary = sal }));
                await _db.SaveChangesAsync();

                // ---------- Employees (with user accounts) ----------
                var names = new[]
                {
                    "Rahim Uddin", "Karim Hossain", "Nusrat Jahan", "Farhana Akter", "Tanvir Ahmed", "Sabbir Rahman", "Mehedi Hasan", "Sadia Islam",
                    "Arif Chowdhury", "Shamima Begum", "Rakib Hasan", "Jannatul Ferdous", "Imran Kabir", "Sumaiya Khatun", "Nayeem Islam", "Fahim Shahriar",
                    "Tasnim Rahman", "Mahmudul Hasan", "Rumana Afroz", "Shakil Ahmed", "Nabila Haque", "Asif Iqbal", "Mim Akter", "Rafiqul Islam"
                };
                // department index, designation index for each person
                var plan = new (int D, int G)[]
                {
                    (0, 0), (0, 1), (0, 2), (0, 2), (0, 2), (0, 1), (1, 0), (1, 3), (1, 3), (2, 0), (2, 4), (2, 4),
                    (3, 0), (3, 5), (3, 5), (3, 5), (3, 5), (4, 0), (4, 6), (4, 6), (0, 2), (2, 4), (3, 5), (4, 6)
                };
                var users = names.Select(n => Add(new User
                {
                    FullName = n,
                    Email = n.ToLowerInvariant().Replace(' ', '.') + DemoDomain,
                    PhoneNumber = "017" + _rnd.Next(10000000, 99999999),
                    RoleId = RoleIds.Employee,
                    PasswordHash = ""
                })).ToList();
                foreach (var u in users) u.PasswordHash = hasher.HashPassword(u, DemoPassword);
                await _db.SaveChangesAsync();

                var codeStart = await _db.Employees.IgnoreQueryFilters().CountAsync() + 1;
                var emps = new List<Employee>();
                for (var i = 0; i < names.Length; i++)
                {
                    var g = desigs[plan[i].G];
                    emps.Add(Add(new Employee
                    {
                        EmployeeCode = (codeStart + i).ToEmployeeCode(),
                        UserId = users[i].UserId,
                        DepartmentId = depts[plan[i].D].DepartmentId,
                        DesignationId = g.DesignationId,
                        BranchId = i % 4 == 3 ? 2 : 1,
                        JoinDate = today.AddDays(-_rnd.Next(60, 365 * 8)),
                        Salary = g.BaseSalary + _rnd.Next(0, 8) * 1000,
                        Status = EmployeeStatus.Active,
                        Address = "Dhaka"
                    }));
                }
                await _db.SaveChangesAsync();
                // Managers: the first "Manager" of each department
                for (var i = 0; i < emps.Count; i++)
                {
                    var manager = emps.FirstOrDefault(e => e.DepartmentId == emps[i].DepartmentId && e.DesignationId == desigs[0].DesignationId);
                    if (manager != null && manager != emps[i]) emps[i].ManagerId = manager.EmployeeId;
                }

                // ---------- Profile data ----------
                var skills = await _db.Skills.ToListAsync();
                foreach (var e in emps)
                {
                    Add(new EmployeeAddress { EmployeeId = e.EmployeeId, AddressType = "Present", AddressLine = $"House {_rnd.Next(1, 90)}, Road {_rnd.Next(1, 30)}", City = "Dhaka", District = "Dhaka" });
                    Add(new EmergencyContact { EmployeeId = e.EmployeeId, Name = "Family contact", Relationship = "Brother", Phone = "018" + _rnd.Next(10000000, 99999999) });
                    Add(new EmployeeEducation { EmployeeId = e.EmployeeId, Degree = "BSc", Institution = "IUBAT", PassingYear = e.JoinDate.Year - 1, Result = "3." + _rnd.Next(10, 90) });
                    Add(new BankAccount { EmployeeId = e.EmployeeId, BankName = "Dutch-Bangla Bank", BranchName = "Uttara", AccountName = users[emps.IndexOf(e)].FullName, AccountNumber = "10" + _rnd.Next(10000000, 99999999), IsPrimary = true });
                    foreach (var sk in skills.OrderBy(_ => _rnd.Next()).Take(2))
                        Add(new EmployeeSkill { EmployeeId = e.EmployeeId, SkillId = sk.SkillId, Proficiency = "Intermediate", YearsOfExperience = _rnd.Next(1, 6) });
                    Add(new EmployeeContract
                    {
                        EmployeeId = e.EmployeeId, EmploymentTypeId = 1, GradeId = GradeFor(e.Salary), ContractNo = "CT-" + e.EmployeeCode,
                        StartDate = e.JoinDate, EndDate = emps.IndexOf(e) % 6 == 0 ? today.AddDays(20 + emps.IndexOf(e)) : null, BasicSalary = e.Salary, Status = "Active"
                    });
                }
                Add(new Dependent { EmployeeId = emps[0].EmployeeId, Name = "Ayesha Uddin", Relationship = "Spouse", Gender = "Female", IsNominee = true, NomineeSharePercent = 100 });

                // ---------- Attendance: last 60 days, Sunday-Thursday ----------
                for (var d = today.AddDays(-60); d <= today; d = d.AddDays(1))
                {
                    if (d.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday) continue;
                    foreach (var e in emps.Where(e => e.JoinDate <= d))
                    {
                        var roll = _rnd.Next(100);
                        if (roll < 6) { Add(new Attendance { EmployeeId = e.EmployeeId, Date = d, Status = AttendanceStatus.Absent }); continue; }
                        var late = roll < 14;
                        var checkIn = new TimeSpan(9, late ? 45 + _rnd.Next(0, 40) : 5 + _rnd.Next(0, 30), 0);
                        Add(new Attendance
                        {
                            EmployeeId = e.EmployeeId, Date = d, CheckIn = checkIn,
                            CheckOut = d == today || roll == 99 ? null : new TimeSpan(18, _rnd.Next(0, 50), 0),
                            Status = late ? AttendanceStatus.Late : AttendanceStatus.Present
                        });
                    }
                }

                // ---------- Leave ----------
                foreach (var e in emps)
                {
                    for (var t = 1; t <= 3; t++)
                        Add(new LeaveBalance { EmployeeId = e.EmployeeId, LeaveTypeId = t, Year = today.Year, Entitled = t == 1 ? 20 : 10, CarriedForward = t == 1 ? 3 : 0, Taken = 0 });
                }
                for (var i = 0; i < 30; i++)
                {
                    var e = emps[_rnd.Next(emps.Count)];
                    var from = today.AddDays(-_rnd.Next(-15, 200));
                    var days = _rnd.Next(1, 4);
                    var status = from > today ? LeaveStatus.Pending : (i % 7 == 0 ? LeaveStatus.Rejected : LeaveStatus.Approved);
                    Add(new LeaveRequest
                    {
                        EmployeeId = e.EmployeeId, LeaveTypeId = _rnd.Next(1, 4), FromDate = from, ToDate = from.AddDays(days - 1), TotalDays = days,
                        Reason = "Personal reason", Status = status, ApprovedById = status == LeaveStatus.Pending ? null : emps[0].EmployeeId,
                        AppliedAt = from.AddDays(i % 5 == 0 ? 0 : -3)
                    });
                }

                // ---------- Payroll: last 2 months + bonus + loans ----------
                var payrolls = new List<Payroll>();
                for (var m = 2; m >= 1; m--)
                {
                    var month = today.AddMonths(-m);
                    foreach (var e in emps)
                    {
                        var allowance = Math.Round(e.Salary * PayrollRules.AllowancePercent / 100, 2);
                        var deduction = Math.Round(e.Salary * PayrollRules.DeductionPercent / 100, 2);
                        payrolls.Add(Add(new Payroll
                        {
                            EmployeeId = e.EmployeeId, Month = month.Month, Year = month.Year, BasicSalary = e.Salary, Allowance = allowance,
                            Deduction = deduction, NetSalary = e.Salary + allowance - deduction, Status = PayrollStatus.Paid, GeneratedAt = month.AddDays(27)
                        }));
                    }
                }
                foreach (var e in emps.Take(12))
                    Add(new Bonus { EmployeeId = e.EmployeeId, BonusType = "Performance", Amount = Math.Round(e.Salary * 0.2m), Month = today.AddMonths(-1).Month, Year = today.AddMonths(-1).Year });
                await _db.SaveChangesAsync();

                for (var i = 0; i < 3; i++)
                {
                    var loan = Add(new LoanAdvance
                    {
                        ReferenceNo = $"LN-DEMO-{i + 1}", EmployeeId = emps[i * 5].EmployeeId, LoanType = i == 2 ? "Advance" : "Loan",
                        Amount = 60000 - i * 15000, Installments = 12, InstallmentAmount = (60000 - i * 15000) / 12m, IssueDate = today.AddMonths(-3), Purpose = "Personal"
                    });
                    await _db.SaveChangesAsync();
                    foreach (var p in payrolls.Where(p => p.EmployeeId == loan.EmployeeId))
                    {
                        Add(new LoanRepayment { LoanAdvanceId = loan.LoanAdvanceId, PayrollId = p.PayrollId, Amount = loan.InstallmentAmount, PaidOn = new DateTime(p.Year, p.Month, 28) });
                        loan.PaidAmount += loan.InstallmentAmount;
                    }
                }

                // ---------- Provident fund ----------
                var pf = await _db.Funds.FirstAsync();
                foreach (var e in emps.Take(14))
                {
                    var member = Add(new FundMembership { FundId = pf.FundId, EmployeeId = e.EmployeeId, JoinDate = e.JoinDate });
                    await _db.SaveChangesAsync();
                    foreach (var p in payrolls.Where(p => p.EmployeeId == e.EmployeeId))
                        Add(new FundContribution { FundMembershipId = member.FundMembershipId, PayrollId = p.PayrollId, Month = p.Month, Year = p.Year, EmployeeAmount = p.BasicSalary * 0.1m, EmployerAmount = p.BasicSalary * 0.1m });
                }

                // ---------- Recruitment ----------
                var jobs = new[]
                {
                    Add(new JobOpening { Title = "Junior .NET Developer", DepartmentId = depts[0].DepartmentId, DesignationId = desigs[2].DesignationId, Vacancies = 2,
                        Description = "Build and maintain ASP.NET Core applications.", Requirements = "C#, ASP.NET Core, SQL Server", SalaryMin = 35000, SalaryMax = 50000,
                        EmploymentType = "Full-time", Deadline = today.AddDays(25), Status = JobStatus.Open, PostedById = admin.UserId }),
                    Add(new JobOpening { Title = "HR Executive", DepartmentId = depts[1].DepartmentId, DesignationId = desigs[3].DesignationId, Vacancies = 1,
                        Description = "Recruitment, onboarding and employee records.", Requirements = "BBA/MBA in HRM, MS Excel", SalaryMin = 30000, SalaryMax = 40000,
                        EmploymentType = "Full-time", Deadline = today.AddDays(15), Status = JobStatus.Open, PostedById = admin.UserId }),
                    Add(new JobOpening { Title = "Sales Executive", DepartmentId = depts[3].DepartmentId, DesignationId = desigs[5].DesignationId, Vacancies = 3,
                        Description = "Field sales and client follow-up.", Requirements = "Good communication, bike riding", SalaryMin = 25000, SalaryMax = 32000,
                        EmploymentType = "Full-time", Deadline = today.AddDays(-5), Status = JobStatus.Closed, PostedById = admin.UserId })
                };
                var candidates = new[] { "Ashik Mahmud", "Lamia Sultana", "Tahmid Hasan", "Priya Das", "Zubair Alam", "Fatema Nasrin", "Rony Barua", "Sharmin Sultana" }
                    .Select((n, i) => Add(new Candidate
                    {
                        FullName = n, Email = n.ToLowerInvariant().Replace(' ', '.') + ".cv" + DemoDomain, Phone = "019" + _rnd.Next(10000000, 99999999),
                        Skills = i % 2 == 0 ? "C#, ASP.NET Core, SQL Server" : "Communication, MS Excel", ExperienceYears = _rnd.Next(0, 6)
                    })).ToList();
                await _db.SaveChangesAsync();
                Add(new JobSkill { JobOpeningId = jobs[0].JobOpeningId, SkillId = skills.First(s => s.Name == "C#").SkillId, IsMandatory = true, MinYears = 1, Weight = 9 });
                Add(new JobSkill { JobOpeningId = jobs[0].JobOpeningId, SkillId = skills.First(s => s.Name == "SQL Server").SkillId, IsMandatory = true, MinYears = 1, Weight = 7 });
                Add(new JobAdvertisement { JobOpeningId = jobs[0].JobOpeningId, RecruitmentSourceId = 1, PostedOn = today.AddDays(-5), ExpiresOn = today.AddDays(25), Cost = 3500 });
                var statuses = new[] { ApplicationStatus.Applied, ApplicationStatus.Shortlisted, ApplicationStatus.Interview, ApplicationStatus.Rejected, ApplicationStatus.Applied, ApplicationStatus.Shortlisted, ApplicationStatus.Applied, ApplicationStatus.Interview };
                var apps = candidates.Select((c, i) => Add(new JobApplication
                {
                    JobOpeningId = jobs[i % 2].JobOpeningId, CandidateId = c.CandidateId, Status = statuses[i], AppliedAt = today.AddDays(-_rnd.Next(1, 10)), CoverLetter = "I am interested in this position."
                })).ToList();
                await _db.SaveChangesAsync();
                foreach (var a in apps.Where(a => a.Status == ApplicationStatus.Interview))
                    Add(new Interview { ApplicationId = a.ApplicationId, InterviewerId = emps[0].EmployeeId, ScheduledAt = today.AddDays(2).AddHours(11), Mode = "Onsite", Location = "Head Office", Status = InterviewStatus.Scheduled });
                Add(new ManpowerRequisition { Title = "Two developers for the CRM project", DepartmentId = depts[0].DepartmentId, DesignationId = desigs[2].DesignationId,
                    RequestedById = emps[0].EmployeeId, NumberOfPositions = 2, RequiredBy = today.AddDays(30), Status = "Approved", JobOpeningId = jobs[0].JobOpeningId, Justification = "New client project" });

                // ---------- Performance and training ----------
                foreach (var e in emps.Skip(1).Take(10))
                {
                    Add(new PerformanceReview { EmployeeId = e.EmployeeId, ReviewerId = emps[0].EmployeeId, Period = "2026 H1", Rating = _rnd.Next(3, 6), Comments = "Good performance.", ReviewedAt = today.AddDays(-40) });
                    Add(new Goal { EmployeeId = e.EmployeeId, ReviewCycleId = 1, Title = "Complete assigned tasks on time", Weight = 40, ProgressPercent = _rnd.Next(20, 100), DueDate = new DateTime(today.Year, 12, 31), Status = "InProgress" });
                }
                var training = Add(new TrainingProgram { Title = "ASP.NET Core Advanced", SkillId = skills.First(s => s.Name == "ASP.NET Core").SkillId, Trainer = "External trainer",
                    Venue = "Head Office", StartDate = today.AddDays(-20), EndDate = today.AddDays(-18), Capacity = 10, Cost = 50000, Status = "Completed" });
                await _db.SaveChangesAsync();
                foreach (var e in emps.Where(e => e.DepartmentId == depts[0].DepartmentId).Take(6))
                    Add(new TrainingEnrollment { TrainingProgramId = training.TrainingProgramId, EmployeeId = e.EmployeeId, Status = "Completed", Score = _rnd.Next(60, 96), CertificateNo = "CERT-" + e.EmployeeCode });

                // ---------- Requests and HRD ----------
                Add(new WorkFromHomeRequest { EmployeeId = emps[2].EmployeeId, FromDate = today.AddDays(1), ToDate = today.AddDays(2), Reason = "Internet work from home", Status = "Pending" });
                Add(new OvertimeRecord { EmployeeId = emps[3].EmployeeId, Date = today.AddDays(-3), Hours = 2.5m, RatePerHour = 300, Status = "Approved" });
                Add(new TravelExpense { EmployeeId = emps[13].EmployeeId, TravelDate = today.AddDays(-6), FromPlace = "Dhaka", ToPlace = "Chattogram", Purpose = "Client meeting",
                    TransportCost = 1500, FoodCost = 600, HotelCost = 2500, TotalAmount = 4600, Status = "Approved", ApprovedById = emps[12].EmployeeId });
                Add(new EmployeePromotion { EmployeeId = emps[4].EmployeeId, FromDesignationId = desigs[2].DesignationId, ToDesignationId = desigs[1].DesignationId,
                    OldSalary = emps[4].Salary, NewSalary = emps[4].Salary + 10000, EffectiveDate = today.AddMonths(1), PromotionType = "Promotion", Status = "Pending" });
                Add(new Grievance { EmployeeId = emps[15].EmployeeId, Subject = "Late salary transfer", Details = "Salary came 3 days late last month.", SubmittedOn = today.AddDays(-4), Status = "InReview", AssignedToId = emps[6].EmployeeId });
                Add(new DisciplinaryAction { EmployeeId = emps[16].EmployeeId, IssuedById = emps[12].EmployeeId, IncidentDate = today.AddDays(-12), ActionType = "VerbalWarning", Description = "Late arrival several days.", Status = "Closed" });
                Add(new HseIncident { IncidentDate = today.AddDays(-9), BranchId = 1, EmployeeId = emps[18].EmployeeId, IncidentType = "NearMiss", Severity = "Low", Description = "Wet floor near the stairs.", ActionTaken = "Warning sign placed.", Status = "Closed" });
                Add(new HrBudget { FinancialYearId = 2, DepartmentId = depts[0].DepartmentId, Category = "Training", BudgetAmount = 300000, SpentAmount = 50000 });
                var survey = Add(new Survey { Title = "Employee satisfaction 2026", StartDate = today.AddDays(-10), EndDate = today.AddDays(10), Status = "Open" });
                await _db.SaveChangesAsync();
                var q1 = Add(new SurveyQuestion { SurveyId = survey.SurveyId, QuestionText = "How happy are you with your work environment?", QuestionType = "Rating", SortOrder = 1 });
                await _db.SaveChangesAsync();
                foreach (var e in emps.Take(8)) Add(new SurveyResponse { SurveyQuestionId = q1.SurveyQuestionId, EmployeeId = e.EmployeeId, Rating = _rnd.Next(3, 6) });
                foreach (var e in emps.Take(10)) Add(new CanteenMeal { EmployeeId = e.EmployeeId, CanteenItemId = 2, Date = today.AddDays(-1), Quantity = 1, Amount = 60 });

                // ---------- Outsourced workers ----------
                var agency = Add(new OutsourcingAgency { Name = "Demo Security Services Ltd.", ContactPerson = "Mr. Kamal", Phone = "01711000000", Address = "Mirpur, Dhaka" });
                await _db.SaveChangesAsync();
                var workers = new[] { "Abdul Malek", "Jamal Mia", "Selina Begum", "Habib Ullah" }.Select(n => Add(new OutsourcedEmployee
                {
                    FullName = n, OutsourcingAgencyId = agency.OutsourcingAgencyId, DepartmentId = depts[4].DepartmentId, BranchId = 1, JobTitle = n == "Selina Begum" ? "Cleaner" : "Security guard",
                    JoinDate = today.AddMonths(-6), DailyRate = 700
                })).ToList();
                await _db.SaveChangesAsync();
                var lastMonth = today.AddMonths(-1);
                foreach (var w in workers)
                {
                    var present = 0;
                    for (var d = new DateTime(lastMonth.Year, lastMonth.Month, 1); d.Month == lastMonth.Month; d = d.AddDays(1))
                    {
                        var ok = _rnd.Next(100) < 92;
                        if (ok) present++;
                        Add(new OutsourcedAttendance { OutsourcedEmployeeId = w.OutsourcedEmployeeId, Date = d, Status = ok ? "Present" : "Absent" });
                    }
                    Add(new OutsourcedSalary { OutsourcedEmployeeId = w.OutsourcedEmployeeId, Month = lastMonth.Month, Year = lastMonth.Year, WorkingDays = present, DailyRate = 700, Amount = present * 700, Status = "Paid" });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return $"Demo data loaded: {emps.Count} employees, about {emps.Count * 40} attendance days, 2 months payroll, 3 jobs, {candidates.Count} candidates. Demo login: any demo e-mail (e.g. {users[1].Email}) / {DemoPassword}";
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            finally
            {
                _db.SkipAudit = false;
                _db.ChangeTracker.Clear();
            }
        }

        private T Add<T>(T entity) where T : class
        {
            _db.Set<T>().Add(entity);
            return entity;
        }

        private static int GradeFor(decimal salary) =>
            salary < 25000 ? 1 : salary < 40000 ? 2 : salary < 60000 ? 3 : salary < 90000 ? 4 : 5;
    }
}
