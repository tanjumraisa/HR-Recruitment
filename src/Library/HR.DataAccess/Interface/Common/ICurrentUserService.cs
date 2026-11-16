namespace HR.DataAccess.Interface
{
    // The logged-in user, used for the audit log and the application status history.
    // The web project registers the real implementation (reads the auth cookie).
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserName { get; }
    }

    public class NoCurrentUserService : ICurrentUserService
    {
        public int? UserId => null;
        public string? UserName => "system";
    }
}
