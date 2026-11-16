using System.Text.Json.Serialization;
using HR.API.Endpoints;
using HR.API.Infrastructure;
using HR.DataAccess.Interface;
using HR.Facade;
using HR.Integration;

// HR.API: JSON REST API over the same Facade layer as the web site.
// No MVC pages here (only the API test page in wwwroot). Every /api call needs the header  X-Api-Key: <Api:Key from appsettings.json>.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddScoped<ICurrentUserService, ApiCurrentUser>();
builder.Services.AddHrFacade(builder.Configuration);
builder.Services.AddHrIntegration();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
if (app.Environment.IsDevelopment()) app.MapOpenApi();   // /openapi/v1.json
app.UseDefaultFiles();   // wwwroot/index.html = API test page
app.UseStaticFiles();
app.UseHttpsRedirection();

var api = app.MapGroup("/api");
api.MapGet("/", () => Results.Ok(EndpointCatalog.Describe())).WithName("Catalog");
api.MapHrQueries();     // dashboard, reports, records, existing modules (read-only)
api.MapHrModules();     // CRUD for every HRMS module table

app.Run();
