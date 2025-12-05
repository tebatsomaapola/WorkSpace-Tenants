using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.Middleware;
using WorkspaceTenants.Services;
using WorkspaceTenants.Infrastructure;
using WorkspaceTenants.Data;

namespace WorkspaceTenants.Controllers{

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInvoiceService _svc;
    public InvoicesController(AppDbContext db, IInvoiceService svc) { _db = db; _svc = svc; }

    [HttpPost("{bookingId:guid}")]
    public async Task<IActionResult> CreateDraft(Guid bookingId, System.Threading.CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();
        var (result, inv) = await _svc.CreateDraftAsync(bookingId, context, ct);
        if (!result.Success) 
        {
           return BadRequest(new { error = result.Error });	
        }
       
        return Ok(inv);
    }

    [HttpGet("tenants-summary")]
    public async Task<IActionResult> TenantSummary([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();

        var confirmed = _db.Bookings.AsNoTracking()
            .Where(b => b.TenantId == context.TenantId && b.Status == "Confirmed" && b.StartUtc >= fromUtc && b.EndUtc <= toUtc);

        var totalHours = await confirmed.SumAsync(
           th => (th.EndUtc - th.StartUtc).TotalHours,
            ct
             );

        var totalInvoiced = await _db.Invoices.AsNoTracking()
            .Where(i => i.TenantId == context.TenantId && i.IssuedUtc >= fromUtc && i.IssuedUtc <= toUtc)
            .SumAsync(i => (decimal?)i.Amount, ct) ?? 0m;

var topRooms = await confirmed
    .GroupBy(b => b.RoomId)
    .Select(g => new
    {
        RoomId = g.Key,
        Hours = g.Sum(b => (b.EndUtc - b.StartUtc).TotalHours)
    })
    .OrderByDescending(x => x.Hours)
    .Take(5)
    .Join(
        _db.Rooms,
        x => x.RoomId,
        r => r.Id,
        (x, r) => new
        {
            r.Id,
            r.Name,
            x.Hours
        }
    )
    .ToListAsync(ct);


        return Ok(new
        {
            TotalHoursBooked = Math.Round(totalHours, 2),
            TotalInvoicedAmount = totalInvoiced,
            TopRooms = topRooms.Select(t => new { t.Id, t.Name, Hours = Math.Round(t.Hours, 2) })
        });
    }

    [HttpGet("user-summary/{userId:guid}")]
    public async Task<IActionResult> UserBillingSummary(Guid userId, [FromQuery] int month, [FromQuery] int year, CancellationToken ct)
    {
        var context = HttpContext.GetTenantContext();
        if (context.Role == "Member" && context.UserId != userId) 
        {
          return Forbid();	
        }
      

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var bookings = _db.Bookings.AsNoTracking()
            .Where(b => b.TenantId == context.TenantId && b.UserId == userId && b.StartUtc >= start && b.EndUtc < end && b.Status == "Confirmed");

        var count = await bookings.CountAsync(ct);
        var avgHours = count == 0 ? 0.0 :
            await bookings.AverageAsync(
               b => (b.EndUtc - b.StartUtc).TotalHours,
               ct
             );

        var totalSpent = await _db.Invoices.AsNoTracking()
            .Where(i => i.TenantId == context.TenantId && i.UserId == userId && i.IssuedUtc >= start && i.IssuedUtc < end)
            .SumAsync(i => (decimal?)i.Amount, ct) ?? 0m;

        return Ok(new { TotalSpent = totalSpent, CountOfBookings = count, AverageBookingDurationHours = Math.Round(avgHours, 2) });
    }
}
}