using System.Globalization;
using HR.DataAccess.Interface;
using HR.DataAccess.Reporting;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    // Read-only queries for the dashboard and the "All Reports" menu.
    public class ReportRepository : IReportRepository
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        private readonly ApplicationDbContext _db;

        public ReportRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        private static string Money(decimal v) => v.ToString("N2", Inv);
        private static string Day(DateTime d) => d.ToString("dd MMM yyyy", Inv);
        private static string Period(int month, int year) => new DateTime(year, month, 1).ToString("MMMM yyyy", Inv);

        public async Task<DashboardStats> GetDashboardAsync()
        {
            var today = DateTime.Today;
            var s = new DashboardStats
            {
                Employees = await _db.Employees.CountAsync(e => e.Status == EmployeeStatus.Active),
                Departments = await _db.Departments.CountAsync(),
                Branches = await _db.Branches.CountAsync(),
                OpenJobs = await _db.JobOpenings.CountAsync(j => j.Status == JobStatus.Open),
                Applications = await _db.JobApplications.CountAsync(),
                PendingLeaves = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Pending),
                PresentToday = await _db.Attendances.CountAsync(a => a.Date == today && (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late)),
                LateToday = await _db.Attendances.CountAsync(a => a.Date == today && a.Status == AttendanceStatus.Late),
                OnLeaveToday = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Approved && l.FromDate <= today && l.ToDate >= today),
                PendingRequests = await _db.WorkFromHomeRequests.CountAsync(x => x.Status == RequestStatus.Pending)
                                  + await _db.AttendanceCorrections.CountAsync(x => x.Status == RequestStatus.Pending)
                                  + await _db.ShiftSwapRequests.CountAsync(x => x.Status == RequestStatus.Pending)
                                  + await _db.OvertimeRecords.CountAsync(x => x.Status == RequestStatus.Pending),
                OpenRequisitions = await _db.ManpowerRequisitions.CountAsync(x => x.Status == RequestStatus.Pending || x.Status == RequestStatus.Approved),
                RunningTrainings = await _db.TrainingPrograms.CountAsync(x => x.Status == "Running" || x.Status == "Planned"),
                ContractsExpiring = await _db.EmployeeContracts.CountAsync(x => x.Status == "Active" && x.EndDate != null && x.EndDate <= today.AddDays(30)),
                RunningLoans = await _db.LoanAdvances.CountAsync(x => x.Status == "Running")
            };

            s.UpcomingHolidays = (await _db.Holidays.Where(h => h.Date >= today).OrderBy(h => h.Date).Take(5)
                .Select(h => new { h.Name, h.Date }).ToListAsync()).Select(h => (h.Name, h.Date)).ToList();

            s.HeadcountByDepartment = await _db.Employees.Where(e => e.Status != EmployeeStatus.Terminated)
                .GroupBy(e => e.Department.Name).Select(g => new { g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count).ToDictionaryAsync(x => x.Key, x => x.Count);
            return s;
        }

        public async Task<List<EmployeeSearchRow>> SearchEmployeesAsync(ReportFilter f)
        {
            var q = _db.Employees.AsNoTracking().AsQueryable();
            if (f.BranchId.HasValue) q = q.Where(e => e.BranchId == f.BranchId);
            if (f.DepartmentId.HasValue) q = q.Where(e => e.DepartmentId == f.DepartmentId);
            if (f.DesignationId.HasValue) q = q.Where(e => e.DesignationId == f.DesignationId);
            if (!string.IsNullOrWhiteSpace(f.Status)) q = q.Where(e => e.Status == f.Status);
            if (!string.IsNullOrWhiteSpace(f.Name)) q = q.Where(e => e.User.FullName.Contains(f.Name));
            if (!string.IsNullOrWhiteSpace(f.EmployeeCode)) q = q.Where(e => e.EmployeeCode.Contains(f.EmployeeCode));

            return await q.OrderBy(e => e.EmployeeCode).Select(e => new EmployeeSearchRow
            {
                EmployeeId = e.EmployeeId,
                Code = e.EmployeeCode,
                Name = e.User.FullName,
                Department = e.Department.Name,
                Designation = e.Designation.Title,
                Branch = e.Branch != null ? e.Branch.Name : "-",
                Status = e.Status,
                JoinDate = e.JoinDate
            }).Take(200).ToListAsync();
        }

        public async Task<ReportResult> EmployeeListAsync(ReportFilter f)
        {
            var rows = await SearchEmployeesAsync(f);
            return new ReportResult
            {
                Title = "Employee List",
                Subtitle = $"{rows.Count} employee(s)",
                Columns = new() { "SL", "Code", "Name", "Department", "Designation", "Branch", "Joined", "Status" },
                Rows = rows.Select((r, i) => new[] { (i + 1).ToString(), r.Code, r.Name, r.Department, r.Designation, r.Branch, Day(r.JoinDate), r.Status }).ToList()
            };
        }

        public async Task<ReportResult> HeadcountAsync()
        {
            var data = await _db.Employees.Where(e => e.Status != EmployeeStatus.Terminated)
                .GroupBy(e => new { Dept = e.Department.Name, Branch = e.Branch != null ? e.Branch.Name : "Not set" })
                .Select(g => new { g.Key.Dept, g.Key.Branch, Total = g.Count(), Active = g.Count(x => x.Status == EmployeeStatus.Active), Salary = g.Sum(x => x.Salary) })
                .OrderBy(x => x.Dept).ThenBy(x => x.Branch).ToListAsync();
            return new ReportResult
            {
                Title = "Headcount by Department and Branch",
                Columns = new() { "Department", "Branch", "Employees", "Active", "Monthly salary" },
                Rows = data.Select(d => new[] { d.Dept, d.Branch, d.Total.ToString(), d.Active.ToString(), Money(d.Salary) }).ToList(),
                Footer = new[] { "Total", "", data.Sum(d => d.Total).ToString(), data.Sum(d => d.Active).ToString(), Money(data.Sum(d => d.Salary)) }
            };
        }

        public async Task<ReportResult> LeaveSummaryAsync(int year)
        {
            var types = await _db.LeaveTypes.OrderBy(t => t.LeaveTypeId).ToListAsync();
            var employees = await _db.Employees.Where(e => e.Status != EmployeeStatus.Terminated)
                .Select(e => new { e.EmployeeId, e.EmployeeCode, e.User.FullName }).OrderBy(e => e.EmployeeCode).ToListAsync();
            var taken = await _db.LeaveRequests.Where(l => l.Status == LeaveStatus.Approved && l.FromDate.Year == year)
                .GroupBy(l => new { l.EmployeeId, l.LeaveTypeId }).Select(g => new { g.Key.EmployeeId, g.Key.LeaveTypeId, Days = g.Sum(x => x.TotalDays) })
                .ToListAsync();
            var balances = await _db.LeaveBalances.Where(b => b.Year == year).ToListAsync();

            var result = new ReportResult { Title = "Leave Summary", Subtitle = "Year " + year };
            result.Columns.Add("Code");
            result.Columns.Add("Name");
            foreach (var t in types) result.Columns.Add(t.Name + " (taken / allowed)");
            foreach (var e in employees)
            {
                var row = new List<string> { e.EmployeeCode, e.FullName };
                foreach (var t in types)
                {
                    var used = taken.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.LeaveTypeId == t.LeaveTypeId)?.Days ?? 0;
                    var bal = balances.FirstOrDefault(b => b.EmployeeId == e.EmployeeId && b.LeaveTypeId == t.LeaveTypeId);
                    var allowed = bal != null ? bal.Entitled + bal.CarriedForward : t.DaysPerYear;
                    row.Add($"{used} / {allowed}");
                }
                result.Rows.Add(row.ToArray());
            }
            return result;
        }

        public async Task<ReportResult> AttendanceSummaryAsync(int month, int year)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var data = await _db.Attendances.Where(a => a.Date >= from && a.Date < to)
                .GroupBy(a => new { a.EmployeeId, a.Employee.EmployeeCode, a.Employee.User.FullName })
                .Select(g => new
                {
                    g.Key.EmployeeCode, g.Key.FullName,
                    Present = g.Count(x => x.Status == AttendanceStatus.Present),
                    Late = g.Count(x => x.Status == AttendanceStatus.Late),
                    Absent = g.Count(x => x.Status == AttendanceStatus.Absent),
                    Leave = g.Count(x => x.Status == AttendanceStatus.OnLeave)
                }).OrderBy(x => x.EmployeeCode).ToListAsync();
            var overtime = await _db.OvertimeRecords.Where(o => o.Date >= from && o.Date < to && o.Status != RequestStatus.Rejected)
                .GroupBy(o => o.Employee.EmployeeCode).Select(g => new { g.Key, Hours = g.Sum(x => x.Hours) }).ToDictionaryAsync(x => x.Key, x => x.Hours);

            return new ReportResult
            {
                Title = "Monthly Attendance Summary",
                Subtitle = Period(month, year),
                Columns = new() { "Code", "Name", "Present", "Late", "Absent", "On leave", "Overtime hours" },
                Rows = data.Select(d => new[] { d.EmployeeCode, d.FullName, d.Present.ToString(), d.Late.ToString(), d.Absent.ToString(), d.Leave.ToString(),
                    (overtime.TryGetValue(d.EmployeeCode, out var h) ? h : 0).ToString("0.##", Inv) }).ToList()
            };
        }

        public async Task<ReportResult> SalarySheetAsync(int month, int year)
        {
            var payrolls = await _db.Payrolls.Where(p => p.Month == month && p.Year == year)
                .Select(p => new { p.PayrollId, p.EmployeeId, p.Employee.EmployeeCode, p.Employee.User.FullName, Dept = p.Employee.Department.Name, p.BasicSalary, p.Allowance, p.Deduction, p.NetSalary, p.Status })
                .OrderBy(p => p.EmployeeCode).ToListAsync();
            var bonuses = await _db.Bonuses.Where(b => b.Month == month && b.Year == year)
                .GroupBy(b => b.EmployeeId).Select(g => new { g.Key, Amount = g.Sum(x => x.Amount) }).ToDictionaryAsync(x => x.Key, x => x.Amount);
            var loans = await _db.LoanRepayments.Where(r => r.PaidOn.Month == month && r.PaidOn.Year == year)
                .GroupBy(r => r.LoanAdvance.EmployeeId).Select(g => new { g.Key, Amount = g.Sum(x => x.Amount) }).ToDictionaryAsync(x => x.Key, x => x.Amount);

            var rows = payrolls.Select(p =>
            {
                var bonus = bonuses.TryGetValue(p.EmployeeId, out var b) ? b : 0;
                var loan = loans.TryGetValue(p.EmployeeId, out var l) ? l : 0;
                var payable = p.NetSalary + bonus - loan;
                return (p, bonus, loan, payable);
            }).ToList();

            return new ReportResult
            {
                Title = "Salary Sheet",
                Subtitle = Period(month, year),
                Columns = new() { "Code", "Name", "Department", "Basic", "Allowance", "Deduction", "Net", "Bonus", "Loan", "Payable", "Status" },
                Rows = rows.Select(r => new[] { r.p.EmployeeCode, r.p.FullName, r.p.Dept, Money(r.p.BasicSalary), Money(r.p.Allowance), Money(r.p.Deduction),
                    Money(r.p.NetSalary), Money(r.bonus), Money(r.loan), Money(r.payable), r.p.Status }).ToList(),
                Footer = new[] { "Total", "", "", Money(rows.Sum(r => r.p.BasicSalary)), Money(rows.Sum(r => r.p.Allowance)), Money(rows.Sum(r => r.p.Deduction)),
                    Money(rows.Sum(r => r.p.NetSalary)), Money(rows.Sum(r => r.bonus)), Money(rows.Sum(r => r.loan)), Money(rows.Sum(r => r.payable)), "" }
            };
        }

        public async Task<ReportResult> RecruitmentFunnelAsync()
        {
            var data = await _db.JobOpenings
                .Select(j => new
                {
                    j.Title, j.Status, j.Vacancies,
                    Total = j.Applications.Count(),
                    Shortlisted = j.Applications.Count(a => a.Status == ApplicationStatus.Shortlisted),
                    Interview = j.Applications.Count(a => a.Status == ApplicationStatus.Interview),
                    Offered = j.Applications.Count(a => a.Status == ApplicationStatus.Offered),
                    Hired = j.Applications.Count(a => a.Status == ApplicationStatus.Hired),
                    Rejected = j.Applications.Count(a => a.Status == ApplicationStatus.Rejected)
                }).OrderBy(j => j.Title).ToListAsync();
            return new ReportResult
            {
                Title = "Recruitment Funnel by Job",
                Columns = new() { "Job", "Status", "Vacancies", "Applications", "Shortlisted", "Interview", "Offered", "Hired", "Rejected" },
                Rows = data.Select(d => new[] { d.Title, d.Status, d.Vacancies.ToString(), d.Total.ToString(), d.Shortlisted.ToString(), d.Interview.ToString(),
                    d.Offered.ToString(), d.Hired.ToString(), d.Rejected.ToString() }).ToList()
            };
        }

        public async Task<ReportResult> TrainingReportAsync()
        {
            var data = await _db.TrainingPrograms
                .Select(t => new
                {
                    t.Title, t.Trainer, t.StartDate, t.EndDate, t.Capacity, t.Cost, t.Status,
                    Enrolled = _db.TrainingEnrollments.Count(e => e.TrainingProgramId == t.TrainingProgramId && e.Status != "Cancelled"),
                    Completed = _db.TrainingEnrollments.Count(e => e.TrainingProgramId == t.TrainingProgramId && e.Status == "Completed"),
                    AvgScore = _db.TrainingEnrollments.Where(e => e.TrainingProgramId == t.TrainingProgramId && e.Score != null).Average(e => (double?)e.Score)
                }).OrderByDescending(t => t.StartDate).ToListAsync();
            return new ReportResult
            {
                Title = "Training Report",
                Columns = new() { "Training", "Trainer", "From", "To", "Capacity", "Enrolled", "Completed", "Average score", "Cost", "Status" },
                Rows = data.Select(d => new[] { d.Title, d.Trainer ?? "-", Day(d.StartDate), Day(d.EndDate), d.Capacity.ToString(), d.Enrolled.ToString(),
                    d.Completed.ToString(), d.AvgScore.HasValue ? d.AvgScore.Value.ToString("0.0", Inv) : "-", Money(d.Cost), d.Status }).ToList()
            };
        }

        public async Task<ReportResult> LoanOutstandingAsync()
        {
            var data = await _db.LoanAdvances
                .Select(l => new { l.ReferenceNo, l.Employee.EmployeeCode, l.Employee.User.FullName, l.LoanType, l.IssueDate, l.Amount, l.PaidAmount, l.InstallmentAmount, l.Status })
                .OrderBy(l => l.EmployeeCode).ToListAsync();
            return new ReportResult
            {
                Title = "Loan and Advance Outstanding",
                Columns = new() { "Reference", "Code", "Name", "Type", "Issued", "Amount", "Paid", "Outstanding", "Installment", "Status" },
                Rows = data.Select(d => new[] { d.ReferenceNo, d.EmployeeCode, d.FullName, d.LoanType, Day(d.IssueDate), Money(d.Amount), Money(d.PaidAmount),
                    Money(d.Amount - d.PaidAmount), Money(d.InstallmentAmount), d.Status }).ToList(),
                Footer = new[] { "Total", "", "", "", "", Money(data.Sum(d => d.Amount)), Money(data.Sum(d => d.PaidAmount)), Money(data.Sum(d => d.Amount - d.PaidAmount)), "", "" }
            };
        }

        public async Task<ReportResult> ExpiryAlertAsync(int days)
        {
            var limit = DateTime.Today.AddDays(days);
            var contracts = await _db.EmployeeContracts.Where(c => c.Status == "Active" && c.EndDate != null && c.EndDate <= limit)
                .Select(c => new { Kind = "Contract", No = c.ContractNo, c.Employee.EmployeeCode, c.Employee.User.FullName, Date = c.EndDate!.Value }).ToListAsync();
            var docs = await _db.EmployeeDocuments.Where(d => d.ExpiryDate != null && d.ExpiryDate <= limit)
                .Select(d => new { Kind = d.DocumentType, No = d.Title, d.Employee.EmployeeCode, d.Employee.User.FullName, Date = d.ExpiryDate!.Value }).ToListAsync();
            var all = contracts.Concat(docs).OrderBy(x => x.Date).ToList();
            return new ReportResult
            {
                Title = "Contract and Document Expiry",
                Subtitle = $"Expired or expiring within {days} days",
                Columns = new() { "Type", "Reference", "Code", "Name", "Expiry date", "Days left" },
                Rows = all.Select(x => new[] { x.Kind, x.No, x.EmployeeCode, x.FullName, Day(x.Date), (x.Date - DateTime.Today).Days.ToString() }).ToList()
            };
        }

        public async Task<ReportResult> OvertimeSummaryAsync(int month, int year)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var data = await _db.OvertimeRecords.Where(o => o.Date >= from && o.Date < to)
                .Select(o => new { o.Employee.EmployeeCode, o.Employee.User.FullName, o.Date, o.Hours, o.RatePerHour, o.Status })
                .OrderBy(o => o.EmployeeCode).ThenBy(o => o.Date).ToListAsync();
            return new ReportResult
            {
                Title = "Overtime Register",
                Subtitle = Period(month, year),
                Columns = new() { "Code", "Name", "Date", "Hours", "Rate", "Amount", "Status" },
                Rows = data.Select(d => new[] { d.EmployeeCode, d.FullName, Day(d.Date), d.Hours.ToString("0.##", Inv), Money(d.RatePerHour), Money(d.Hours * d.RatePerHour), d.Status }).ToList(),
                Footer = new[] { "Total", "", "", data.Sum(d => d.Hours).ToString("0.##", Inv), "", Money(data.Sum(d => d.Hours * d.RatePerHour)), "" }
            };
        }
    
        public async Task<ReportResult> NewJoinersAsync(int month, int year)
        {
            var data = await _db.Employees.Where(e => e.JoinDate.Month == month && e.JoinDate.Year == year)
                .Select(e => new { e.EmployeeCode, e.User.FullName, Dept = e.Department.Name, Desig = e.Designation.Title, e.JoinDate, e.Salary })
                .OrderBy(e => e.JoinDate).ToListAsync();
            return new ReportResult
            {
                Title = "Employee New Join List", Subtitle = Period(month, year),
                Columns = new() { "Code", "Name", "Department", "Designation", "Join date", "Salary" },
                Rows = data.Select(d => new[] { d.EmployeeCode, d.FullName, d.Dept, d.Desig, Day(d.JoinDate), Money(d.Salary) }).ToList()
            };
        }

        public async Task<ReportResult> IncrementReportAsync(int year)
        {
            var data = await _db.EmployeePromotions.Where(p => p.EffectiveDate.Year == year)
                .Select(p => new { p.Employee.EmployeeCode, p.Employee.User.FullName, p.PromotionType, To = p.ToDesignation.Title, p.OldSalary, p.NewSalary, p.EffectiveDate, p.Status })
                .OrderBy(p => p.EffectiveDate).ToListAsync();
            return new ReportResult
            {
                Title = "Increment and Promotion Report", Subtitle = "Year " + year,
                Columns = new() { "Code", "Name", "Type", "New designation", "Old salary", "New salary", "Increase", "Effective", "Status" },
                Rows = data.Select(d => new[] { d.EmployeeCode, d.FullName, d.PromotionType, d.To, Money(d.OldSalary), Money(d.NewSalary),
                    Money(d.NewSalary - d.OldSalary), Day(d.EffectiveDate), d.Status }).ToList()
            };
        }

        public async Task<ReportResult> PunchMissingAsync(int month, int year)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var data = await _db.Attendances.Where(a => a.Date >= from && a.Date < to && a.Status != AttendanceStatus.Absent && a.Status != AttendanceStatus.OnLeave
                                                        && (a.CheckIn == null || a.CheckOut == null))
                .Select(a => new { a.Employee.EmployeeCode, a.Employee.User.FullName, a.Date, a.CheckIn, a.CheckOut })
                .OrderBy(a => a.Date).ThenBy(a => a.EmployeeCode).ToListAsync();
            return new ReportResult
            {
                Title = "In-punch and Out-punch Missing Report", Subtitle = Period(month, year),
                Columns = new() { "Date", "Code", "Name", "In punch", "Out punch" },
                Rows = data.Select(d => new[] { Day(d.Date), d.EmployeeCode, d.FullName,
                    d.CheckIn.HasValue ? d.CheckIn.Value.ToString(@"hh\:mm") : "MISSING", d.CheckOut.HasValue ? d.CheckOut.Value.ToString(@"hh\:mm") : "MISSING" }).ToList()
            };
        }

        public async Task<ReportResult> FundBalanceAsync()
        {
            var data = await _db.FundMemberships
                .Select(m => new
                {
                    Fund = m.Fund.Name, m.Employee.EmployeeCode, m.Employee.User.FullName, m.Status,
                    Paid = _db.FundContributions.Where(c => c.FundMembershipId == m.FundMembershipId).Sum(c => (decimal?)(c.EmployeeAmount + c.EmployerAmount)) ?? 0,
                    Taken = _db.FundWithdrawals.Where(w => w.FundMembershipId == m.FundMembershipId && (w.Status == "Approved" || w.Status == "Paid")).Sum(w => (decimal?)w.Amount) ?? 0
                }).OrderBy(x => x.Fund).ThenBy(x => x.EmployeeCode).ToListAsync();
            return new ReportResult
            {
                Title = "Fund Balance Report",
                Columns = new() { "Fund", "Code", "Name", "Contributions", "Withdrawn", "Balance", "Status" },
                Rows = data.Select(d => new[] { d.Fund, d.EmployeeCode, d.FullName, Money(d.Paid), Money(d.Taken), Money(d.Paid - d.Taken), d.Status }).ToList(),
                Footer = new[] { "Total", "", "", Money(data.Sum(d => d.Paid)), Money(data.Sum(d => d.Taken)), Money(data.Sum(d => d.Paid - d.Taken)), "" }
            };
        }

        public async Task<ReportResult> OutsourcedSalaryAsync(int month, int year)
        {
            var data = await _db.OutsourcedSalaries.Where(s => s.Month == month && s.Year == year)
                .Select(s => new { s.OutsourcedEmployee.FullName, Agency = s.OutsourcedEmployee.OutsourcingAgency.Name, s.WorkingDays, s.DailyRate, s.Amount, s.Status })
                .OrderBy(s => s.Agency).ThenBy(s => s.FullName).ToListAsync();
            return new ReportResult
            {
                Title = "Outsourced Employee Salary Sheet", Subtitle = Period(month, year),
                Columns = new() { "Name", "Agency", "Present days", "Daily rate", "Amount", "Status" },
                Rows = data.Select(d => new[] { d.FullName, d.Agency, d.WorkingDays.ToString(), Money(d.DailyRate), Money(d.Amount), d.Status }).ToList(),
                Footer = new[] { "Total", "", data.Sum(d => d.WorkingDays).ToString(), "", Money(data.Sum(d => d.Amount)), "" }
            };
        }

        public async Task<ReportResult> TravelExpenseAsync(int month, int year)
        {
            var data = await _db.TravelExpenses.Where(t => t.TravelDate.Month == month && t.TravelDate.Year == year)
                .Select(t => new { t.Employee.EmployeeCode, t.Employee.User.FullName, t.TravelDate, t.FromPlace, t.ToPlace, t.Purpose, t.TotalAmount, t.Status })
                .OrderBy(t => t.TravelDate).ToListAsync();
            return new ReportResult
            {
                Title = "TA/DA Expense Report", Subtitle = Period(month, year),
                Columns = new() { "Date", "Code", "Name", "From", "To", "Purpose", "Amount", "Status" },
                Rows = data.Select(d => new[] { Day(d.TravelDate), d.EmployeeCode, d.FullName, d.FromPlace, d.ToPlace, d.Purpose, Money(d.TotalAmount), d.Status }).ToList(),
                Footer = new[] { "Total", "", "", "", "", "", Money(data.Sum(d => d.TotalAmount)), "" }
            };
        }

        public async Task<DashboardCharts> GetDashboardChartsAsync(DateTime date, int? departmentId)
        {
            date = date.Date;
            var employees = _db.Employees.Where(e => e.Status != EmployeeStatus.Terminated);
            var attendance = _db.Attendances.AsQueryable();
            var leaves = _db.LeaveRequests.AsQueryable();
            if (departmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId);
                attendance = attendance.Where(a => a.Employee.DepartmentId == departmentId);
                leaves = leaves.Where(l => l.Employee.DepartmentId == departmentId);
            }

            var c = new DashboardCharts { Date = date };
            c.TotalEmployees = await employees.CountAsync();
            var today = await attendance.Where(a => a.Date == date).Select(a => a.Status).ToListAsync();
            c.DaysPresent = today.Count(s => s == AttendanceStatus.Present || s == AttendanceStatus.Late);
            c.DaysAbsent = today.Count(s => s == AttendanceStatus.Absent);
            // Leave taken on the same day it was applied for = unscheduled
            c.UnscheduledLeave = await leaves.CountAsync(l => l.FromDate <= date && l.ToDate >= date && l.AppliedAt.Date >= l.FromDate);

            var yearLeaves = await leaves.Where(l => l.Status == LeaveStatus.Approved && l.FromDate.Year == date.Year)
                .GroupBy(l => l.LeaveType.Name).Select(g => new { g.Key, Days = g.Sum(x => x.TotalDays) }).ToListAsync();
            c.SickLeaveDays = yearLeaves.Where(x => x.Key.Contains("Sick")).Sum(x => x.Days);
            c.CasualLeaveDays = yearLeaves.Where(x => x.Key.Contains("Casual")).Sum(x => x.Days);
            c.LeaveTypes = yearLeaves.OrderByDescending(x => x.Days).Select(x => (x.Key, x.Days)).ToList();

            var marked = c.DaysPresent + c.DaysAbsent;
            c.AbsenteeismRate = marked == 0 ? 0 : Math.Round(c.DaysAbsent * 100m / marked, 1);
            c.PresentismRate = c.TotalEmployees == 0 ? 0 : Math.Round(c.DaysPresent * 100m / c.TotalEmployees, 1);

            var joins = await employees.Select(e => e.JoinDate).ToListAsync();
            double Years(DateTime j) => (date - j).TotalDays / 365.25;
            c.ExperienceBuckets = new List<(string, int)>
            {
                ("0-1 Years", joins.Count(j => Years(j) < 1)),
                ("1-3 Years", joins.Count(j => Years(j) >= 1 && Years(j) < 3)),
                ("3-5 Years", joins.Count(j => Years(j) >= 3 && Years(j) < 5)),
                ("5-7 Years", joins.Count(j => Years(j) >= 5 && Years(j) < 7)),
                ("7+ Years", joins.Count(j => Years(j) >= 7))
            };

            var from = new DateTime(date.Year, date.Month, 1).AddMonths(-11);
            var absents = await attendance.Where(a => a.Date >= from && a.Status == AttendanceStatus.Absent)
                .GroupBy(a => new { a.Date.Year, a.Date.Month }).Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() }).ToListAsync();
            for (var m = from; m <= date; m = m.AddMonths(1))
                c.AbsencesByMonth.Add((m.ToString("MMM", Inv), absents.FirstOrDefault(a => a.Year == m.Year && a.Month == m.Month)?.Count ?? 0));

            c.WorkLocations = (await employees.GroupBy(e => e.Branch != null ? e.Branch.Name : "Not set")
                .Select(g => new { g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToListAsync())
                .Select(x => (x.Key, x.Count)).ToList();

            var depts = await employees.GroupBy(e => new { e.DepartmentId, e.Department.Name }).Select(g => new { g.Key.DepartmentId, g.Key.Name, Count = g.Count() }).ToListAsync();
            var presentByDept = await attendance.Where(a => a.Date == date && (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late))
                .GroupBy(a => a.Employee.DepartmentId).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
            c.AttendanceByDepartment = depts.OrderBy(d => d.Name)
                .Select(d => (d.Name, d.Count == 0 ? 0m : Math.Round((presentByDept.TryGetValue(d.DepartmentId, out var p) ? p : 0) * 100m / d.Count, 1))).ToList();
            return c;
        }
}
}
