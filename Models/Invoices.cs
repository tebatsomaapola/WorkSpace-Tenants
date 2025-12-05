using System;

namespace WorkspaceTenants.Models{

public class Invoices
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Paid
}
}