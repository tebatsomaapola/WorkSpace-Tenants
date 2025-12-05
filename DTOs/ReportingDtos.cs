using System;

namespace WorkspaceTenants.DTOs{

public record TenantUsageSummary(decimal TotalHoursBooked, decimal TotalInvoicedAmount, IReadOnlyList<(Guid RoomId, string RoomName, decimal Hours)> TopRooms);
public record UserBillingSummary(decimal TotalSpent, int CountBookings, double AverageDurationHours);
}