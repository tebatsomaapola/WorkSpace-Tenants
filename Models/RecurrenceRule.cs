using System;

namespace WorkspaceTenants.Models{

public class RecurrenceRule
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string Pattern { get; set; } = ""; // e.g. "Weekly-Tue-10:00-11:00-Count=4"
}
}