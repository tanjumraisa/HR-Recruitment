using HR.Facade.Interface;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    // Notifications of the logged-in user (bell icon in the top bar).
    [Authorize]
    public class NotificationsController : AppController
    {
        private readonly INotificationFacade _notifications;

        public NotificationsController(INotificationFacade notifications)
        {
            _notifications = notifications;
        }

        public async Task<IActionResult> Index() => View(await _notifications.GetMineAsync(User.GetUserId(), 100));

        public async Task<IActionResult> Open(int id)
        {
            var n = await _notifications.OpenAsync(id, User.GetUserId());
            if (n == null) return NotFound();
            return !string.IsNullOrEmpty(n.Link) && Url.IsLocalUrl(n.Link) ? Redirect(n.Link) : RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notifications.MarkAllReadAsync(User.GetUserId());
            TempData["Success"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }
    }

    // Read-only list of the automatic audit trail.
    [Authorize(Roles = HR.Framework.RoleNames.AdminOnly)]
    public class AuditLogsController : AppController
    {
        private readonly IAuditLogFacade _audit;

        public AuditLogsController(IAuditLogFacade audit)
        {
            _audit = audit;
        }

        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            const int pageSize = 30;
            var (items, total) = await _audit.SearchAsync(q, page, pageSize);
            ViewBag.Q = q;
            ViewBag.Page = Math.Max(1, page);
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            ViewBag.Total = total;
            return View(items);
        }
    }
}
