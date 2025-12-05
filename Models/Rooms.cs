using System;
using System.Collections.Generic;


namespace WorkspaceTenants.Models{

public class Rooms
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; } = 1;
    public decimal HourlyRate { get; set; } = 100m;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
}
}
