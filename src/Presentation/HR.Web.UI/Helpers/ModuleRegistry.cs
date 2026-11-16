using System.Security.Claims;
using HR.Framework;

namespace HR.Web.UI.Helpers
{
    public record ModuleInfo(string Entity, string Controller, string Title, string Group, string Description, string Roles = "");

    public record MenuGroup(string Key, string Title);

    // All screens of the system, grouped like the menu of an HRM package
    // (Employee Information, Recruitment, Attendance & Leave, Payroll, HRD Activities, Settings).
    public static partial class ModuleRegistry
    {
        public static readonly MenuGroup[] Groups =
        {
            new("Employee", "Employee Information"),
            new("Outsourced", "Outsourced Employee"),
            new("Recruitment", "Recruitment"),
            new("Leave", "Attendance & Leave"),
            new("Payroll", "Payroll Management"),
            new("Fund", "Fund Management"),
            new("Hrd", "HRD Activities"),
            new("Settings", "Software Settings")
        };

        // Modules that existed before the HRMS extension (own controllers and views).
        private static readonly ModuleInfo[] Existing =
        {
            new("Employee", "Employees", "Employees", "Employee", "Employee master list", RoleNames.HrAdmin),
            new("JobOpening", "Jobs", "Job openings", "Recruitment", "Vacancies published on the careers page", RoleNames.Staff),
            new("Candidate", "Candidates", "Candidates", "Recruitment", "Candidate profiles and CVs", RoleNames.Staff),
            new("JobApplication", "Applications", "Applications", "Recruitment", "Applications and hiring pipeline", RoleNames.Staff),
            new("Interview", "Interviews", "Interviews", "Recruitment", "Interview schedule", RoleNames.Staff),
            new("OfferLetter", "Offers", "Offer letters", "Recruitment", "Offers (accept = hire)", RoleNames.Staff),
            new("Attendance", "Attendance", "Attendance", "Leave", "Daily check in / check out", RoleNames.HrAdminEmployee),
            new("LeaveRequest", "Leaves", "Leave requests", "Leave", "Leave applications", RoleNames.HrAdminEmployee),
            new("Payroll", "Payroll", "Payroll", "Payroll", "Monthly payroll and payslips", RoleNames.HrAdminEmployee),
            new("PerformanceReview", "Reviews", "Performance reviews", "Hrd", "Appraisal ratings", RoleNames.HrAdminEmployee),
            new("Department", "Departments", "Departments", "Settings", "Departments", RoleNames.HrAdmin),
            new("Designation", "Designations", "Designations", "Settings", "Designations and base salary", RoleNames.HrAdmin),
            new("LeaveType", "LeaveTypes", "Leave types", "Settings", "Leave types", RoleNames.HrAdmin),
            new("User", "Users", "Users", "Settings", "User accounts and roles", RoleNames.AdminOnly)
        };

        private static List<ModuleInfo>? _all;

        public static IReadOnlyList<ModuleInfo> All => _all ??= Existing.Concat(Generated
            .Where(g => g.Entity != "Notification")
            .Select(g => g with
            {
                Title = FixTitle(g.Title),
                Roles = g.Entity == "AuditLog" ? RoleNames.AdminOnly
                      : g.Group == "Recruitment" ? RoleNames.Staff : RoleNames.HrAdmin,
                Group = g.Group == "System" ? "Settings" : g.Group
            })).ToList();

        // "Hr budgets" -> "HR budgets", "Hse incidents" -> "HSE incidents" ...
        private static string FixTitle(string t) =>
            t.Replace("Hr ", "HR ").Replace("Hse ", "HSE ").Replace("Ta/da", "TA/DA");

        public static ModuleInfo? ForEntity(string entityName) =>
            All.FirstOrDefault(m => m.Entity.Equals(entityName, StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<ModuleInfo> InGroup(string group, ClaimsPrincipal user) =>
            All.Where(m => m.Group == group && IsAllowed(user, m.Roles));

        public static bool IsAllowed(ClaimsPrincipal user, string roles) =>
            string.IsNullOrEmpty(roles) || roles.Split(',').Any(user.IsInRole);
    }
}
