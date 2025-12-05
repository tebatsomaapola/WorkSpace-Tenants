using System;

namespace WorkspaceTenants.Models{

public class Bookings
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled
    public Guid? RecurrenceSeriesId { get; set; } // group future bookings
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
}