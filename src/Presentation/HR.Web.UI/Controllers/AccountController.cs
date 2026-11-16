using System.Security.Claims;
using HR.Facade.Interface;
using HR.Framework;
using HR.Web.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.UI.Controllers
{
    public class AccountController : AppController
    {
        private readonly IUserFacade _userFacade;

        public AccountController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        [AllowAnonymous]
        [HttpGet(UrlRewrite.Account.Login)]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost(UrlRewrite.Account.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userFacade.AuthenticateAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.RoleName)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(model.RememberMe ? 24 * 7 : 2)
                });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return user.Role.RoleName == RoleNames.Candidate
                ? RedirectToAction("MyApplications", "Careers")
                : RedirectToAction("Index", "Dashboard");
        }

        [AllowAnonymous]
        [HttpGet(UrlRewrite.Account.Register)]
        public IActionResult Register() => View(new RegisterViewModel());

        [AllowAnonymous]
        [HttpPost(UrlRewrite.Account.Register)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _userFacade.RegisterCandidateAsync(model.FullName, model.Email, model.Phone, model.Password);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            TempData["Success"] = "Account created. Please log in.";
            return Redirect("/" + UrlRewrite.Account.Login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/" + UrlRewrite.Account.Login);
        }

        [AllowAnonymous]
        [HttpGet(UrlRewrite.Account.AccessDenied)]
        public IActionResult AccessDenied() => View();
    }
}
