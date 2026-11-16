using System.Linq.Expressions;
using HR.Entities;
using HR.Framework;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Designation> Designations { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<JobOpening> JobOpenings { get; set; } = null!;
        public DbSet<Candidate> Candidates { get; set; } = null!;
        public DbSet<JobApplication> JobApplications { get; set; } = null!;
        public DbSet<Interview> Interviews { get; set; } = null!;
        public DbSet<OfferLetter> OfferLetters { get; set; } = null!;
        public DbSet<LeaveType> LeaveTypes { get; set; } = null!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Payroll> Payrolls { get; set; } = null!;
        public DbSet<PerformanceReview> PerformanceReviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Unique indexes ----------
            modelBuilder.Entity<Role>().HasIndex(x => x.RoleName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
            // Unique rules ignore soft-deleted rows, so a deleted name / email / month can be used again.
            const string Active = "[IsDeleted] = 0";
            modelBuilder.Entity<Department>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Designation>().HasIndex(x => x.Title).IsUnique().HasFilter(Active);
            modelBuilder.Entity<LeaveType>().HasIndex(x => x.Name).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeCode).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Employee>().HasIndex(x => x.UserId).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Candidate>().HasIndex(x => x.Email).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Candidate>().HasIndex(x => x.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL AND [IsDeleted] = 0");
            modelBuilder.Entity<JobApplication>().HasIndex(x => new { x.JobOpeningId, x.CandidateId }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<OfferLetter>().HasIndex(x => x.ApplicationId).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Attendance>().HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique().HasFilter(Active);
            modelBuilder.Entity<Payroll>().HasIndex(x => new { x.EmployeeId, x.Month, x.Year }).IsUnique().HasFilter(Active);

            // ---------- Users, roles, profiles ----------
            modelBuilder.Entity<User>()
                .HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.User).WithOne(u => u.Employee).HasForeignKey<Employee>(x => x.UserId);

            modelBuilder.Entity<Candidate>()
                .HasOne(x => x.User).WithOne(u => u.Candidate).HasForeignKey<Candidate>(x => x.UserId);

            // ---------- Employee ----------
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Department).WithMany(d => d.Employees).HasForeignKey(x => x.DepartmentId);
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Designation).WithMany(d => d.Employees).HasForeignKey(x => x.DesignationId);
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Manager).WithMany(m => m.Subordinates).HasForeignKey(x => x.ManagerId);

            // ---------- Recruitment ----------
            modelBuilder.Entity<JobOpening>()
                .HasOne(x => x.Department).WithMany(d => d.JobOpenings).HasForeignKey(x => x.DepartmentId);
            modelBuilder.Entity<JobOpening>()
                .HasOne(x => x.Designation).WithMany(d => d.JobOpenings).HasForeignKey(x => x.DesignationId);
            modelBuilder.Entity<JobOpening>()
                .HasOne(x => x.PostedBy).WithMany().HasForeignKey(x => x.PostedById);

            modelBuilder.Entity<JobApplication>()
                .HasOne(x => x.JobOpening).WithMany(j => j.Applications).HasForeignKey(x => x.JobOpeningId);
            modelBuilder.Entity<JobApplication>()
                .HasOne(x => x.Candidate).WithMany(c => c.Applications).HasForeignKey(x => x.CandidateId);

            modelBuilder.Entity<Interview>()
                .HasOne(x => x.Application).WithMany(a => a.Interviews).HasForeignKey(x => x.ApplicationId);
            modelBuilder.Entity<Interview>()
                .HasOne(x => x.Interviewer).WithMany(e => e.Interviews).HasForeignKey(x => x.InterviewerId);

            modelBuilder.Entity<OfferLetter>()
                .HasOne(x => x.Application).WithOne(a => a.Offer).HasForeignKey<OfferLetter>(x => x.ApplicationId);

            // ---------- Leave, attendance, payroll, performance ----------
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(x => x.Employee).WithMany(e => e.LeaveRequests).HasForeignKey(x => x.EmployeeId);
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(x => x.LeaveType).WithMany(t => t.LeaveRequests).HasForeignKey(x => x.LeaveTypeId);
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(x => x.ApprovedBy).WithMany().HasForeignKey(x => x.ApprovedById);

            modelBuilder.Entity<Attendance>()
                .HasOne(x => x.Employee).WithMany(e => e.Attendances).HasForeignKey(x => x.EmployeeId);

            modelBuilder.Entity<Payroll>()
                .HasOne(x => x.Employee).WithMany(e => e.Payrolls).HasForeignKey(x => x.EmployeeId);

            modelBuilder.Entity<PerformanceReview>()
                .HasOne(x => x.Employee).WithMany(e => e.PerformanceReviews).HasForeignKey(x => x.EmployeeId);
            modelBuilder.Entity<PerformanceReview>()
                .HasOne(x => x.Reviewer).WithMany().HasForeignKey(x => x.ReviewerId);

            // ---------- Seed data ----------
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = RoleIds.Admin, RoleName = RoleNames.Admin },
                new Role { RoleId = RoleIds.HRManager, RoleName = RoleNames.HRManager },
                new Role { RoleId = RoleIds.Recruiter, RoleName = RoleNames.Recruiter },
                new Role { RoleId = RoleIds.Employee, RoleName = RoleNames.Employee },
                new Role { RoleId = RoleIds.Candidate, RoleName = RoleNames.Candidate });

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { LeaveTypeId = 1, Name = "Annual Leave", DaysPerYear = 20 },
                new LeaveType { LeaveTypeId = 2, Name = "Sick Leave", DaysPerYear = 10 },
                new LeaveType { LeaveTypeId = 3, Name = "Casual Leave", DaysPerYear = 10 });

            // ---------- Soft delete: hide rows with IsDeleted = true from every query ----------
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var notDeleted = Expression.Equal(Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)), Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(notDeleted, parameter));
            }

            // ---------- Delete behaviour ----------
            // SQL Server rejects multiple cascade paths, and HR data should never disappear silently,
            // so every foreign key is Restrict. Facades turn the resulting DbUpdateException into a friendly message.
            // Keep this loop LAST so it also covers the relationships configured above.
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
