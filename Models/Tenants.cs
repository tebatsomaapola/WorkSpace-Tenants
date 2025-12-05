using System;
using System.Collections.Generic;

namespace WorkspaceTenants.Models{

public class Tenants
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    public ICollection<UserAccount> Users { get; set; } = new List<UserAccount>();
}
}
