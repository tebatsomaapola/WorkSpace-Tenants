using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.DTOs;
using WorkspaceTenants.Common;
using WorkspaceTenants.Models;
using System;
using WorkspaceTenants.Data;
using System.Threading;
using System.Threading.Tasks;
using WorkspaceTenants.Infrastructure;

namespace WorkspaceTenants.Services{

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly PricingService _pricing;

    public BookingService(AppDbContext db, PricingService pricing)
    {
        _db = db; _pricing = pricing;
    }

    public async Task<(Result result, BookingResponse? booking)> CreateAsync(CreateBookingRequest req, ITenantContext context, CancellationToken ct)
    {
        if (req.StartUtc >= req.EndUtc)
            return (Result.Fail("Start must be before End."), null);

        var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == req.RoomId && r.TenantId == context.TenantId, ct);
        if (room is null) return (Result.Fail("Room not found."), null);

        // Overlap check (Confirmed bookings only)
        bool overlap(Bookings b) => b.Status == "Confirmed"
                                   && b.RoomId == req.RoomId
                                   && b.TenantId == context.TenantId
                                   && req.StartUtc < b.EndUtc
                                   && req.EndUtc > b.StartUtc;

        // High-contention safe create with retry (optimistic concurrency)
        async Task<Bookings> createSingle(DateTime s, DateTime e)
        {
            var conflict = await _db.Bookings.AnyAsync(b => overlap(b) && b.StartUtc < e && b.EndUtc > s, ct);
            if (conflict)
            {
              throw new InvalidOperationException("Time slot already booked.");
	
            }
            
            var booking = new Bookings
            {
                Id = Guid.NewGuid(),
                TenantId = context.TenantId,
                WorkspaceId = req.WorkspaceId,
                RoomId = req.RoomId,
                UserId = context.UserId,
                StartUtc = s,
                EndUtc = e,
                Status = "Confirmed"
            };
            booking.Price = _pricing.Calculate(room, booking, context.TenantId);
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync(ct);
            return booking;
        }

        Bookings created;
        if (!req.Recurring)
        {
            // simple retry
            for (int i = 0; i < 3; i++)
            {
                try { created = await createSingle(req.StartUtc, req.EndUtc); break; }
                catch (DbUpdateConcurrencyException) when (i < 2) { await Task.Delay(50, ct); continue; }
                catch (InvalidOperationException ex) { return (Result.Fail(ex.Message), null); }
            }
            return (Result.Ok(), new BookingResponse(created.Id, created.RoomId, created.StartUtc, created.EndUtc, created.Price, created.Status, null));
        }
        else
        {
            // Weekly recurrence example (e.g., every Tuesday for N weeks)
            var seriesId = Guid.NewGuid();
            int count = req.RecurrenceCount ?? 4;

            var currentStart = req.StartUtc;
            var currentEnd = req.EndUtc;

            for (int n = 0; n < count; n++)
            {
                Bookings b;
                try
                {
                    b = await createSingle(currentStart, currentEnd);
                    b.RecurrenceSeriesId = seriesId;
                    await _db.SaveChangesAsync(ct);
                }
                catch (InvalidOperationException ex) { return (Result.Fail($"Conflict in occurrence {n + 1}: {ex.Message}"), null); }
                currentStart = currentStart.AddDays(7);
                currentEnd = currentEnd.AddDays(7);
            }

            var first = await _db.Bookings.FirstAsync(b => b.RecurrenceSeriesId == seriesId && b.StartUtc == req.StartUtc, ct);
            return (Result.Ok(), new BookingResponse(first.Id, first.RoomId, first.StartUtc, first.EndUtc, first.Price, first.Status, seriesId));
        }
    }

    public async Task<Result> CancelAsync(Guid bookingId, bool cancelSeriesFuture, ITenantContext context, CancellationToken ct)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.TenantId == context.TenantId, ct);
        if (booking is null) return Result.Fail("Not found.");

        if (context.Role == "Member" && booking.UserId != context.UserId)
            return Result.Fail("Forbidden: you can only cancel your own bookings.");

        if (!cancelSeriesFuture || booking.RecurrenceSeriesId is null)
        {
            booking.Status = "Cancelled";
            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }

        var now = booking.StartUtc;
        var seriesId = booking.RecurrenceSeriesId.Value;
        var future = await _db.Bookings
            .Where(b => b.TenantId == context.TenantId && b.RecurrenceSeriesId == seriesId && b.StartUtc >= now)
            .ToListAsync(ct);

        foreach (var b in future) b.Status = "Cancelled";
        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
}
