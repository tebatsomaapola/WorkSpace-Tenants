using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorkspaceTenants.Services;
using System.Threading.Tasks;


namespace WorkspaceTenants.Middleware{

public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;
    public TenantContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            context.Items["TenantContext"] = new TenantContext(context.User);
        }
        await _next(context);
    }
}

public static class TenantContextExtensions
{
    public static ITenantContext GetTenantContext(this HttpContext context)
        => (ITenantContext)context.Items["TenantContext"]!;
}
}