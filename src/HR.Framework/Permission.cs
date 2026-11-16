namespace HR.Framework
{
    // Role names are used in [Authorize(Roles = ...)] and in claims.
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string HRManager = "HRManager";
        public const string Recruiter = "Recruiter";
        public const string Employee = "Employee";
        public const string Candidate = "Candidate";

        // Comma separated groups for [Authorize(Roles = RoleNames.HrAdmin)]
        public const string HrAdmin = "Admin,HRManager";
        public const string Staff = "Admin,HRManager,Recruiter";
        public const string AdminOnly = "Admin";
        public const string HrAdminEmployee = "Admin,HRManager,Employee";
    }

    // Must match the seed data in ApplicationDbContext.
    public static class RoleIds
    {
        public const int Admin = 1;
        public const int HRManager = 2;
        public const int Recruiter = 3;
        public const int Employee = 4;
        public const int Candidate = 5;
    }
}
