using System;
using System.Collections.Generic;


namespace WorkspaceTenants.Models{

public class Workspaces
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = "";
    public string Location { get; set; } = "";
    public ICollection<Rooms> Rooms { get; set; } = new List<Rooms>();
}
}
