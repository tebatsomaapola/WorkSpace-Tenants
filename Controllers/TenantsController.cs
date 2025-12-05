using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.Middleware;
using WorkspaceTenants.Infrastructure;
using WorkspaceTenants.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace WorkspaceTenants.Controllers{

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TenantsController(AppDbContext db) => _db = db;

    [HttpGet("workspaces")]
    public async Task<IActionResult> Workspaces(CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();
        var list = await _db.Workspaces.AsNoTracking()
            .Where(w => w.TenantId == context.TenantId)
            .Select(w => new {
                w.Id, w.Name, w.Location,
                Rooms = _db.Rooms.Where(r => r.WorkspaceId == w.Id).Select(r => new { r.Id, r.Name, r.Capacity, r.HourlyRate })
            })
            .ToListAsync(ct);
        return Ok(list);
    }
}
}