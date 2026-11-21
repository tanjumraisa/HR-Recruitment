using System.Security.Claims;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace HR.Web.UI.Filters
{
    // Checks the user permission (Permission / PermissionGroup / UserPermission tables) on every request.
    //  - Admin: always allowed.
    //  - User with no group / permission assigned: only the role decides (old behaviour).
    //  - User with groups / permissions: may open only the screens and buttons that are ticked.
    public class PermissionFilter : IAsyncActionFilter
    {
        public const string ItemKey = "UserPermissions";
        private static int _version;
        private readonly IPermissionFacade _permissions;
        private readonly IMemoryCache _cache;

        public PermissionFilter(IPermissionFacade permissions, IMemoryCache cache)
        {
            _permissions = permissions;
            _cache = cache;
        }

        // Call after permissions change so every user gets the new list on the next click.
        public static void ClearCache() => Interlocked.Increment(ref _version);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true && !user.IsInRole(RoleNames.Admin)
                && int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                var set = await _cache.GetOrCreateAsync($"perm:{userId}:{_version}", e =>
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return _permissions.GetUserPermissionNamesAsync(userId);
                });
                context.HttpContext.Items[ItemKey] = set;

                var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                var action = context.RouteData.Values["action"]?.ToString() ?? "";
                var needed = PermissionCatalog.Required(controller, action);
                if (set != null && needed != null && !set.Contains(needed))
                {
                    context.Result = new RedirectResult("/" + UrlRewrite.Account.AccessDenied);
                    return;
                }
            }
            await next();
        }
    }

    public static class PermissionExtensions
    {
        // For views: true when the logged-in user may do this (e.g. Html.Can("Employee.Create")).
        public static bool Can(this HttpContext http, string permission) =>
            http.User.IsInRole(RoleNames.Admin)
            || http.Items[PermissionFilter.ItemKey] is not HashSet<string> set
            || set.Contains(permission);
    }
}
