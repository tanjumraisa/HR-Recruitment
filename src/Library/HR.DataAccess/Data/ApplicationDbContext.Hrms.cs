using HR.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess
{
    // HRMS module tables (organization, employee profile, recruitment, leave, payroll, training, system).
    public partial class ApplicationDbContext
    {
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Grade> Grades { get; set; } = null!;
        public DbSet<EmploymentType> EmploymentTypes { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Holiday> Holidays { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<EmployeeShift> EmployeeShifts { get; set; } = null!;
        public DbSet<JobCategory> JobCategories { get; set; } = null!;
        public DbSet<EmployeeAddress> EmployeeAddresses { get; set; } = null!;
        public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;
        public DbSet<EmployeeEducation> EmployeeEducations { get; set; } = null!;
        public DbSet<EmployeeExperience> EmployeeExperiences { get; set; } = null!;
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; } = null!;
        public DbSet<Dependent> Dependents { get; set; } = null!;
        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
        public DbSet<EmployeeContract> EmployeeContracts { get; set; } = null!;
        public DbSet<ManpowerRequisition> ManpowerRequisitions { get; set; } = null!;
        public DbSet<RecruitmentSource> RecruitmentSources { get; set; } = null!;
        public DbSet<JobAdvertisement> JobAdvertisements { get; set; } = null!;
        public DbSet<JobSkill> JobSkills { get; set; } = null!;
        public DbSet<CandidateSkill> CandidateSkills { get; set; } = null!;
        public DbSet<CandidateEducation> CandidateEducations { get; set; } = null!;
        public DbSet<CandidateExperience> CandidateExperiences { get; set; } = null!;
        public DbSet<CandidateDocument> CandidateDocuments { get; set; } = null!;
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; } = null!;
        public DbSet<InterviewPanel> InterviewPanels { get; set; } = null!;
        public DbSet<InterviewFeedback> InterviewFeedbacks { get; set; } = null!;
        public DbSet<LeavePolicy> LeavePolicies { get; set; } = null!;
        public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
        public DbSet<OvertimeRecord> OvertimeRecords { get; set; } = null!;
        public DbSet<AttendanceCorrection> AttendanceCorrections { get; set; } = null!;
        public DbSet<CompensatoryLeave> CompensatoryLeaves { get; set; } = null!;
        public DbSet<WorkFromHomeRequest> WorkFromHomeRequests { get; set; } = null!;
        public DbSet<ShiftSwapRequest> ShiftSwapRequests { get; set; } = null!;
        public DbSet<SalaryComponent> SalaryComponents { get; set; } = null!;
        public DbSet<EmployeeSalaryComponent> EmployeeSalaryComponents { get; set; } = null!;
        public DbSet<PayrollItem> PayrollItems { get; set; } = null!;
        public DbSet<Bonus> Bonuses { get; set; } = null!;
        public DbSet<LoanAdvance> LoanAdvances { get; set; } = null!;
        public DbSet<LoanRepayment> LoanRepayments { get; set; } = null!;
        public DbSet<TaxSlab> TaxSlabs { get; set; } = null!;
        public DbSet<ReviewCycle> ReviewCycles { get; set; } = null!;
        public DbSet<Goal> Goals { get; set; } = null!;
        public DbSet<TrainingProgram> TrainingPrograms { get; set; } = null!;
        public DbSet<TrainingEnrollment> TrainingEnrollments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<EmployeeTransfer> EmployeeTransfers { get; set; } = null!;
        public DbSet<EmployeePromotion> EmployeePromotions { get; set; } = null!;
        public DbSet<TravelExpense> TravelExpenses { get; set; } = null!;
        public DbSet<LeaveWithdrawal> LeaveWithdrawals { get; set; } = null!;
        public DbSet<OutsourcingAgency> OutsourcingAgencies { get; set; } = null!;
        public DbSet<OutsourcedEmployee> OutsourcedEmployees { get; set; } = null!;
        public DbSet<OutsourcedAttendance> OutsourcedAttendances { get; set; } = null!;
        public DbSet<OutsourcedSalary> OutsourcedSalaries { get; set; } = null!;
        public DbSet<Fund> Funds { get; set; } = null!;
        public DbSet<FundMembership> FundMemberships { get; set; } = null!;
        public DbSet<FundContribution> FundContributions { get; set; } = null!;
        public DbSet<FundWithdrawal> FundWithdrawals { get; set; } = null!;
        public DbSet<DisciplinaryAction> DisciplinaryActions { get; set; } = null!;
        public DbSet<Grievance> Grievances { get; set; } = null!;
        public DbSet<LabourCase> LabourCases { get; set; } = null!;
        public DbSet<Survey> Surveys { get; set; } = null!;
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; } = null!;
        public DbSet<SurveyResponse> SurveyResponses { get; set; } = null!;
        public DbSet<CanteenItem> CanteenItems { get; set; } = null!;
        public DbSet<CanteenMeal> CanteenMeals { get; set; } = null!;
        public DbSet<FinancialYear> FinancialYears { get; set; } = null!;
        public DbSet<HrBudget> HrBudgets { get; set; } = null!;
        public DbSet<HseIncident> HseIncidents { get; set; } = null!;
        public DbSet<HrAudit> HrAudits { get; set; } = null!;
        public DbSet<FestivalBonusSetup> FestivalBonusSetups { get; set; } = null!;

        private static void ConfigureHrmsModules(ModelBuilder modelBuilder)
        {
            const string Active = "[IsDeleted] = 0";

            // Unique rules (soft-deleted rows are ignored)
            modelBuilder.Entity<Branch>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Grade>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<EmploymentType>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Shift>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Team>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<TeamMember>().HasIndex(x => new { x.TeamId, x.EmployeeId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<JobCategory>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Skill>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<EmployeeSkill>().HasIndex(x => new { x.EmployeeId, x.SkillId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<RecruitmentSource>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<JobAdvertisement>().HasIndex(x => new { x.JobOpeningId, x.RecruitmentSourceId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<JobSkill>().HasIndex(x => new { x.JobOpeningId, x.SkillId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<CandidateSkill>().HasIndex(x => new { x.CandidateId, x.SkillId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<InterviewPanel>().HasIndex(x => new { x.InterviewId, x.EmployeeId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<InterviewFeedback>().HasIndex(x => new { x.InterviewId, x.EmployeeId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<LeaveBalance>().HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<SalaryComponent>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<EmployeeSalaryComponent>().HasIndex(x => new { x.EmployeeId, x.SalaryComponentId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<PayrollItem>().HasIndex(x => new { x.PayrollId, x.SalaryComponentId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<ReviewCycle>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<TrainingEnrollment>().HasIndex(x => new { x.TrainingProgramId, x.EmployeeId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<OutsourcingAgency>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<OutsourcedAttendance>().HasIndex(x => new { x.OutsourcedEmployeeId, x.Date }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<OutsourcedSalary>().HasIndex(x => new { x.OutsourcedEmployeeId, x.Month, x.Year }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Fund>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<FundMembership>().HasIndex(x => new { x.FundId, x.EmployeeId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<FundContribution>().HasIndex(x => new { x.FundMembershipId, x.Month, x.Year }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<LabourCase>().HasIndex(x => x.CaseNo).IsUnique().HasFilter(Active);
            modelBuilder.Entity<CanteenItem>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<FinancialYear>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<HrBudget>().HasIndex(x => new { x.FinancialYearId, x.DepartmentId, x.Category }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Branch>().HasIndex(x => x.Code).IsUnique().HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");
            modelBuilder.Entity<EmployeeContract>().HasIndex(x => x.ContractNo).IsUnique().HasFilter(Active);
            modelBuilder.Entity<LoanAdvance>().HasIndex(x => x.ReferenceNo).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Notification>().HasIndex(x => new { x.UserId, x.IsRead });
            modelBuilder.Entity<AuditLog>().HasIndex(x => x.CreatedAt);

            HrmsSeed.Seed(modelBuilder);
        }
    }
}
