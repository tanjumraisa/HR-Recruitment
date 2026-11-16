using System.Text.Json;
using HR.DataAccess.Interface;
using HR.Framework;

namespace HR.API.Infrastructure
{
    // Changes made through the API appear in the audit log as "api-client".
    public class ApiCurrentUser : ICurrentUserService
    {
        public int? UserId => null;
        public string? UserName => "api-client";
    }

    // Simple shared-key protection for /api. The test page and /openapi stay open.
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string? _key;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _key = config["Api:Key"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (string.IsNullOrEmpty(_key) || context.Request.Headers["X-Api-Key"] != _key)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Missing or wrong X-Api-Key header." });
                    return;
                }
            }
            await _next(context);
        }
    }

    // Business rule errors -> 400 with the message, anything else -> 500 without details.
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _log;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (BadHttpRequestException ex) when (ex.InnerException is JsonException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid JSON: " + ex.InnerException.Message });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unhandled API error");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Server error. See the API log." });
            }
        }
    }

}
