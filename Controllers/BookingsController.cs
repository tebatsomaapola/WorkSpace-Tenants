using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.Middleware;
using WorkspaceTenants.DTOs;
using WorkspaceTenants.Data;
using WorkspaceTenants.Services;
using WorkspaceTenants.Common;
using WorkspaceTenants.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkspaceTenants.Controllers{

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IBookingService _svc;

    public BookingsController(AppDbContext db, IBookingService svc) { _db = db; _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest req, CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();
        if (context.Role == "ReadOnly") 
        {
           return Forbid();
        }
        
        var (result, booking) = await _svc.CreateAsync(req, context, ct);
        if (!result.Success) 
        {
         return Conflict(new { error = result.Error });
	
        }
        
        return Ok(booking);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingRequest body, CancellationToken ct)
    {
        var ctx = HttpContext.GetTenantContext();
        var res = await _svc.CancelAsync(id, body.CancelSeriesFuture, ctx, ct);
        if (!res.Success) 
        {
          return BadRequest(new { error = res.Error });	
        }
      
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var context = HttpContext.GetTenantContext();
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 10, 200);

        var query = _db.Bookings.AsNoTracking()
            .Where(b => b.TenantId == context.TenantId)
            .OrderByDescending(b => b.StartUtc);

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(items);
    }
}
}
