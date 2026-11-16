using HR.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess
{
    // Starting data for the setup tables, so the dropdowns are not empty on a new database.
    // All values can be edited or deleted later from Software Settings.
    public static class HrmsSeed
    {
        public static void Seed(ModelBuilder b)
        {
            b.Entity<Branch>().HasData(
                new Branch { BranchId = 1, Name = "Head Office", Code = "HO", Address = "Dhaka", IsHeadOffice = true },
                new Branch { BranchId = 2, Name = "Chattogram Branch", Code = "CTG", Address = "Chattogram" });

            b.Entity<Grade>().HasData(
                new Grade { GradeId = 1, Name = "Grade 1", Level = 1, MinSalary = 15000, MaxSalary = 25000 },
                new Grade { GradeId = 2, Name = "Grade 2", Level = 2, MinSalary = 25000, MaxSalary = 40000 },
                new Grade { GradeId = 3, Name = "Grade 3", Level = 3, MinSalary = 40000, MaxSalary = 60000 },
                new Grade { GradeId = 4, Name = "Grade 4", Level = 4, MinSalary = 60000, MaxSalary = 90000 },
                new Grade { GradeId = 5, Name = "Grade 5", Level = 5, MinSalary = 90000, MaxSalary = 150000 });

            b.Entity<EmploymentType>().HasData(
                new EmploymentType { EmploymentTypeId = 1, Name = "Permanent" },
                new EmploymentType { EmploymentTypeId = 2, Name = "Probation" },
                new EmploymentType { EmploymentTypeId = 3, Name = "Contractual" },
                new EmploymentType { EmploymentTypeId = 4, Name = "Intern" },
                new EmploymentType { EmploymentTypeId = 5, Name = "Part-time" });

            b.Entity<Shift>().HasData(
                new Shift { ShiftId = 1, Name = "General", StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(18, 0, 0), GraceMinutes = 10 },
                new Shift { ShiftId = 2, Name = "Morning", StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(14, 0, 0), GraceMinutes = 10 },
                new Shift { ShiftId = 3, Name = "Evening", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0), GraceMinutes = 10 },
                new Shift { ShiftId = 4, Name = "Night", StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(6, 0, 0), GraceMinutes = 10, IsNightShift = true });

            b.Entity<JobCategory>().HasData(
                new JobCategory { JobCategoryId = 1, Name = "IT and Software" },
                new JobCategory { JobCategoryId = 2, Name = "Accounts and Finance" },
                new JobCategory { JobCategoryId = 3, Name = "Human Resources" },
                new JobCategory { JobCategoryId = 4, Name = "Sales and Marketing" },
                new JobCategory { JobCategoryId = 5, Name = "Administration" });

            b.Entity<Skill>().HasData(
                new Skill { SkillId = 1, Name = "C#", Category = "Technical" },
                new Skill { SkillId = 2, Name = "ASP.NET Core", Category = "Technical" },
                new Skill { SkillId = 3, Name = "SQL Server", Category = "Technical" },
                new Skill { SkillId = 4, Name = "JavaScript", Category = "Technical" },
                new Skill { SkillId = 5, Name = "Python", Category = "Technical" },
                new Skill { SkillId = 6, Name = "MS Excel", Category = "Technical" },
                new Skill { SkillId = 7, Name = "Accounting", Category = "Domain" },
                new Skill { SkillId = 8, Name = "Communication", Category = "Soft" },
                new Skill { SkillId = 9, Name = "Leadership", Category = "Soft" },
                new Skill { SkillId = 10, Name = "English", Category = "Language" });

            b.Entity<RecruitmentSource>().HasData(
                new RecruitmentSource { RecruitmentSourceId = 1, Name = "Bdjobs", SourceType = "JobPortal", Website = "https://www.bdjobs.com" },
                new RecruitmentSource { RecruitmentSourceId = 2, Name = "LinkedIn", SourceType = "SocialMedia", Website = "https://www.linkedin.com" },
                new RecruitmentSource { RecruitmentSourceId = 3, Name = "Facebook", SourceType = "SocialMedia" },
                new RecruitmentSource { RecruitmentSourceId = 4, Name = "Employee referral", SourceType = "Referral" },
                new RecruitmentSource { RecruitmentSourceId = 5, Name = "University job fair", SourceType = "Campus" },
                new RecruitmentSource { RecruitmentSourceId = 6, Name = "Company careers page", SourceType = "WalkIn" });

            b.Entity<SalaryComponent>().HasData(
                new SalaryComponent { SalaryComponentId = 1, Name = "House Rent", ComponentType = "Earning", CalculationType = "PercentOfBasic", DefaultValue = 50 },
                new SalaryComponent { SalaryComponentId = 2, Name = "Medical Allowance", ComponentType = "Earning", CalculationType = "PercentOfBasic", DefaultValue = 10 },
                new SalaryComponent { SalaryComponentId = 3, Name = "Conveyance", ComponentType = "Earning", CalculationType = "Fixed", DefaultValue = 1500 },
                new SalaryComponent { SalaryComponentId = 4, Name = "Provident Fund", ComponentType = "Deduction", CalculationType = "PercentOfBasic", DefaultValue = 10 },
                new SalaryComponent { SalaryComponentId = 5, Name = "Income Tax", ComponentType = "Deduction", CalculationType = "Fixed", DefaultValue = 0 },
                new SalaryComponent { SalaryComponentId = 6, Name = "Loan Installment", ComponentType = "Deduction", CalculationType = "Fixed", DefaultValue = 0 });

            // Sample slabs for testing; update them to the current tax rules before real use.
            b.Entity<TaxSlab>().HasData(
                new TaxSlab { TaxSlabId = 1, Description = "First slab", FiscalYear = "2026-27", FromAmount = 0, ToAmount = 350000, RatePercent = 0 },
                new TaxSlab { TaxSlabId = 2, Description = "Second slab", FiscalYear = "2026-27", FromAmount = 350001, ToAmount = 450000, RatePercent = 5 },
                new TaxSlab { TaxSlabId = 3, Description = "Third slab", FiscalYear = "2026-27", FromAmount = 450001, ToAmount = 850000, RatePercent = 10 },
                new TaxSlab { TaxSlabId = 4, Description = "Fourth slab", FiscalYear = "2026-27", FromAmount = 850001, ToAmount = 1350000, RatePercent = 15 },
                new TaxSlab { TaxSlabId = 5, Description = "Fifth slab", FiscalYear = "2026-27", FromAmount = 1350001, ToAmount = 999999999, RatePercent = 20 });

            b.Entity<Holiday>().HasData(
                new Holiday { HolidayId = 1, Name = "International Mother Language Day", Date = new DateTime(2026, 2, 21), HolidayType = "Public" },
                new Holiday { HolidayId = 2, Name = "Independence Day", Date = new DateTime(2026, 3, 26), HolidayType = "Public" },
                new Holiday { HolidayId = 3, Name = "Bengali New Year", Date = new DateTime(2026, 4, 14), HolidayType = "Public" },
                new Holiday { HolidayId = 4, Name = "May Day", Date = new DateTime(2026, 5, 1), HolidayType = "Public" },
                new Holiday { HolidayId = 5, Name = "Victory Day", Date = new DateTime(2026, 12, 16), HolidayType = "Public" },
                new Holiday { HolidayId = 6, Name = "International Mother Language Day", Date = new DateTime(2027, 2, 21), HolidayType = "Public" });

            b.Entity<ReviewCycle>().HasData(
                new ReviewCycle { ReviewCycleId = 1, Name = "Annual Review 2026", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Status = "Open" });

            b.Entity<FinancialYear>().HasData(
                new FinancialYear { FinancialYearId = 1, Name = "2025-26", StartDate = new DateTime(2025, 7, 1), EndDate = new DateTime(2026, 6, 30) },
                new FinancialYear { FinancialYearId = 2, Name = "2026-27", StartDate = new DateTime(2026, 7, 1), EndDate = new DateTime(2027, 6, 30), IsCurrent = true });

            b.Entity<Fund>().HasData(
                new Fund { FundId = 1, Name = "Provident Fund", FundType = "ProvidentFund", EmployeeContributionPercent = 10, EmployerContributionPercent = 10 },
                new Fund { FundId = 2, Name = "Gratuity Fund", FundType = "Gratuity", EmployeeContributionPercent = 0, EmployerContributionPercent = 8.33m },
                new Fund { FundId = 3, Name = "Workers' Welfare Fund", FundType = "WorkersWelfare", EmployeeContributionPercent = 0, EmployerContributionPercent = 0.5m });

            b.Entity<CanteenItem>().HasData(
                new CanteenItem { CanteenItemId = 1, Name = "Breakfast", MealType = "Breakfast", UnitPrice = 60, SubsidyPercent = 50 },
                new CanteenItem { CanteenItemId = 2, Name = "Lunch", MealType = "Lunch", UnitPrice = 120, SubsidyPercent = 50 },
                new CanteenItem { CanteenItemId = 3, Name = "Evening snacks", MealType = "Snacks", UnitPrice = 40, SubsidyPercent = 0 });

            b.Entity<FestivalBonusSetup>().HasData(
                new FestivalBonusSetup { FestivalBonusSetupId = 1, Festival = "EidUlFitr", CalculationType = "PercentOfBasic", Value = 100, MinServiceMonths = 6 },
                new FestivalBonusSetup { FestivalBonusSetupId = 2, Festival = "EidUlAdha", CalculationType = "PercentOfBasic", Value = 100, MinServiceMonths = 6 });

            b.Entity<LeavePolicy>().HasData(
                new LeavePolicy { LeavePolicyId = 1, LeaveTypeId = 1, DaysPerYear = 20, CarryForwardMax = 10, IsEncashable = true, MinServiceMonths = 12 },
                new LeavePolicy { LeavePolicyId = 2, LeaveTypeId = 2, DaysPerYear = 10, CarryForwardMax = 0 },
                new LeavePolicy { LeavePolicyId = 3, LeaveTypeId = 3, DaysPerYear = 10, CarryForwardMax = 0 });
        }
    }
}
