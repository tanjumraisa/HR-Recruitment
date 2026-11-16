using System.Security.Claims;
using HR.Facade.Interface;
using HR.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationFacade _notifications;

        public NotificationBellViewComponent(INotificationFacade notifications)
        {
            _notifications = notifications;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = ((ClaimsPrincipal)User).GetUserId();
            var count = userId > 0 ? await _notifications.CountUnreadAsync(userId) : 0;
            return View(count);
        }
    }
}
