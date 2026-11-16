using HR.DataAccess;
using HR.DataAccess.Implementation;
using HR.DataAccess.Interface;
using HR.Facade.Implementation;
using HR.Facade.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Facade
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHrFacade(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleFacade, RoleFacade>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFacade, UserFacade>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentFacade, DepartmentFacade>();

            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IDesignationFacade, DesignationFacade>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeFacade, EmployeeFacade>();

            services.AddScoped<IJobOpeningRepository, JobOpeningRepository>();
            services.AddScoped<IJobOpeningFacade, JobOpeningFacade>();

            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<ICandidateFacade, CandidateFacade>();

            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IJobApplicationFacade, JobApplicationFacade>();

            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<IInterviewFacade, InterviewFacade>();

            services.AddScoped<IOfferLetterRepository, OfferLetterRepository>();
            services.AddScoped<IOfferLetterFacade, OfferLetterFacade>();

            services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            services.AddScoped<ILeaveTypeFacade, LeaveTypeFacade>();

            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<ILeaveRequestFacade, LeaveRequestFacade>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IAttendanceFacade, AttendanceFacade>();

            services.AddScoped<IPayrollRepository, PayrollRepository>();
            services.AddScoped<IPayrollFacade, PayrollFacade>();

            services.AddScoped<IPerformanceReviewRepository, PerformanceReviewRepository>();
            services.AddScoped<IPerformanceReviewFacade, PerformanceReviewFacade>();

            return services;
        }
    }
}
