using System;
using System.Collections.Generic;
using WorkspaceTenants.Models;

namespace WorkspaceTenants.Data{

public class InMemoryStore
{
    public List<UserAccount> Users { get; } = new();
    public List<Workspace> Workspaces { get; } = new();
    public List<Rooms> Room { get; } = new();
    public List<Bookings> Bookings { get; } = new();
    public List<Invoices> Invoices { get; } = new();

    public InMemoryStore()
    {
        // Seed demo data
        var user = new UserAccount { Email = "mashianyanam@gmail.com", Password = "password", Name = "Demo User" };
        Users.Add(user);

        var wSpace1 = new Workspace { Name = "City Hub", Location = "CBD" };
        var wSpace2 = new Workspace { Name = "Tech Loft", Location = "Midtown" };
        Workspaces.AddRange(new[] { wSpace1, wSpace2 });

        var room1 = new Rooms { Name = "Room A", Capacity = 4, HourlyRate = 150m, WorkspaceId = wSpace1.Id };
        var room2 = new Rooms { Name = "Room B", Capacity = 8, HourlyRate = 300m, WorkspaceId = wSpace2.Id };
        var room3 = new Rooms { Name = "Studio 1", Capacity = 2, HourlyRate = 100m, WorkspaceId = wSpace2.Id };
        Room.AddRange(new[] { room1, room2, room3 });

        wSpace1.Room.AddRange(new[] { room1, room2 });
        wSpace2.Room.AddRange(new[] { room3 });
    }
}
}
