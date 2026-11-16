using HR.DataAccess.Interface;

namespace HR.Web.UI.Helpers
{
    // Gives the data layer the logged-in user (for the audit log and status history).
    public class HttpCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public HttpCurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public int? UserId
        {
            get
            {
                var user = _http.HttpContext?.User;
                return user?.Identity?.IsAuthenticated == true ? user.GetUserId() : null;
            }
        }

        public string? UserName => _http.HttpContext?.User?.Identity?.Name ?? "system";
    }
}
