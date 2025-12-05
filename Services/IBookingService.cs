using WorkspaceTenants.DTOs;
using WorkspaceTenants.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkspaceTenants.Services{

public interface IBookingService
{
    Task<(Result result, BookingResponse? booking)> CreateAsync(CreateBookingRequest req, ITenantContext context, CancellationToken ct);
    Task<Result> CancelAsync(Guid bookingId, bool cancelSeriesFuture, ITenantContext context, CancellationToken ct);
}
}