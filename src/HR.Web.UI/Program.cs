using HR.DataAccess;
using HR.Facade;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC. Non-nullable navigation properties must not be implicitly [Required] when entities are posted from forms.
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    // Every page needs a logged-in user unless it is marked [AllowAnonymous].
    options.Filters.Add(new AuthorizeFilter());
});

// Dependency injection: DbContext + every Repository and Facade
builder.Services.AddHrFacade(builder.Configuration);

// Authentication (cookie) + Authorization (roles)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/" + UrlRewrite.Account.Login;
        options.AccessDeniedPath = "/" + UrlRewrite.Account.AccessDenied;
        options.Cookie.Name = "HR.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations and make sure the first admin exists.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Applies the migrations (creates the database and tables when they do not exist yet).
    await db.Database.MigrateAsync();

    var seed = builder.Configuration.GetSection("SeedAdmin");
    var users = scope.ServiceProvider.GetRequiredService<IUserFacade>();
    await users.EnsureAdminAsync(seed["FullName"] ?? "System Admin", seed["Email"] ?? "admin@hr.local", seed["Password"] ?? "Admin@12345");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Careers}/{action=Index}/{id?}");

app.Run();