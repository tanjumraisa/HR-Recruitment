using HR.Framework;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    public abstract class AppController : Controller
    {
        // Runs a business operation, shows a success / error message and redirects.
        protected async Task<IActionResult> TryAsync(Func<Task> work, string successMessage, string action = "Index")
        {
            try
            {
                await work();
                TempData["Success"] = successMessage;
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(action);
        }
    }
}
