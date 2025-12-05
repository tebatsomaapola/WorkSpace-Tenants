using WorkspaceTenants.Models;
using WorkspaceTenants.Data;
using System;
namespace WorkspaceTenants.Seed{

public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.Tenants.Any()) return;

        var tenant = new Tenants { Id = Guid.NewGuid(), Name = "Tenant X" };
        db.Tenants.Add(tenant);

        var ws = new Workspaces { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Main Hub", Location = "CBD" };
        db.Workspaces.Add(ws);

        var r1 = new Rooms { Id = Guid.NewGuid(), TenantId = tenant.Id, WorkspaceId = ws.Id, Name = "Room A", Capacity = 6, HourlyRate = 200 };
        var r2 = new Rooms { Id = Guid.NewGuid(), TenantId = tenant.Id, WorkspaceId = ws.Id, Name = "Room B", Capacity = 10, HourlyRate = 300 };
        db.Rooms.AddRange(r1, r2);

        var admin = new UserAccount { Id = Guid.NewGuid(), TenantId = tenant.Id, Email = "admin@tenantx.test", PasswordHash = "password", Name = "Admin X", Role = "TenantAdmin" };
        var member = new UserAccount { Id = Guid.NewGuid(), TenantId = tenant.Id, Email = "member@tenantx.test", PasswordHash = "password", Name = "Member X", Role = "Member" };
        db.Users.AddRange(admin, member);

        db.SaveChanges();
    }
}
}
