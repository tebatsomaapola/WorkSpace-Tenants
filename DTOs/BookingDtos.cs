using System;

namespace WorkspaceTenants.DTOs{

public record CreateBookingRequest(Guid WorkspaceId, Guid RoomId, DateTime StartUtc, DateTime EndUtc, bool Recurring, string? RecurrencePattern, int? RecurrenceCount);
public record BookingResponse(Guid Id, Guid RoomId, DateTime StartUtc, DateTime EndUtc, decimal Price, string Status, Guid? SeriesId);
public record CancelBookingRequest(bool CancelSeriesFuture);
}