using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.Middleware;
using WorkspaceTenants.DTOs;
using WorkspaceTenants.Data;
using WorkspaceTenants.Infrastructure;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace WorkspaceTenants{

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RoomsController(AppDbContext db) => _db = db;

    [HttpGet("availability")]
    public async Task<ActionResult<IEnumerable<RoomAvailabilityItem>>> Availability(
        [FromQuery] Guid workspaceId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        [FromQuery] int? minCapacity,
        [FromQuery] decimal? maxRate,
        CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();
        var rooms = _db.Rooms.AsNoTracking()
            .Where(r => r.TenantId == context.TenantId && r.WorkspaceId == workspaceId);

        if (minCapacity.HasValue) rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        if (maxRate.HasValue) rooms = rooms.Where(r => r.HourlyRate <= maxRate.Value);

        // Exclude rooms with overlapping confirmed bookings
        var bookedRoomIds = await _db.Bookings.AsNoTracking()
            .Where(b => b.TenantId == context.TenantId && b.Status == "Confirmed"
                        && fromUtc < b.EndUtc && toUtc > b.StartUtc)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync(ct);

        var available = await rooms
            .Where(r => !bookedRoomIds.Contains(r.Id))
            .Select(r => new RoomAvailabilityItem(r.Id, r.Name, r.Capacity, r.HourlyRate))
            .ToListAsync(ct);

        return Ok(available);
    }
}
}