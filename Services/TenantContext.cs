using System;
using System.Security.Claims;


namespace WorkspaceTenants.Services{

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; }
    public Guid UserId { get; }
    public string Role { get; } = "";

    public TenantContext(ClaimsPrincipal user)
    {
        TenantId = Guid.Parse(user.FindFirstValue("tenantId")!);
        UserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Role = user.FindFirstValue(ClaimTypes.Role) ?? "";
    }
}
}
