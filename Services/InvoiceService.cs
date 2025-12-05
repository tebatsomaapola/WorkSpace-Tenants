using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.DTOs;
using WorkspaceTenants.Data;
using WorkspaceTenants.Common;
using WorkspaceTenants.Models;
using WorkspaceTenants.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkspaceTenants.Services{

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;
    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<(Result result, InvoiceResponse? invoice)> CreateDraftAsync(Guid bookingId, ITenantContext context, CancellationToken ct)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.TenantId == context.TenantId, ct);
        if (booking is null) return (Result.Fail("Booking not found."), null);

        var invoice = new Invoices
        {
            Id = Guid.NewGuid(),
            TenantId = context.TenantId,
            BookingId = booking.Id,
            UserId = booking.UserId,
            Amount = booking.Price,
            Status = "Draft"
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(ct);

        return (Result.Ok(), new InvoiceResponse(invoice.Id, invoice.Amount, invoice.Status));
    }
}
}
