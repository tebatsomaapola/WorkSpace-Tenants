using System;
using Microsoft.EntityFrameworkCore;
using WorkspaceTenants.Models;

namespace WorkspaceTenants.Data{

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tenants> Tenants => Set<Tenants>();
    public DbSet<Workspaces> Workspaces => Set<Workspaces>();
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Bookings> Bookings => Set<Bookings>();
    public DbSet<Invoices> Invoices => Set<Invoices>();
    public DbSet<RecurrenceRule> RecurrenceRules => Set<RecurrenceRule>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        
        b.Entity<Workspaces>().HasIndex(x => new { x.TenantId });
        b.Entity<Rooms>().HasIndex(x => new { x.TenantId, x.WorkspaceId });

        
        b.Entity<Rooms>().Property(x => x.RowVersion).IsRowVersion();
        b.Entity<Bookings>().Property(x => x.RowVersion).IsRowVersion();

        
        
        b.Entity<Bookings>().HasIndex(x => new { x.TenantId, x.RoomId, x.StartUtc, x.EndUtc }).IsUnique();

        base.OnModelCreating(b);
    }
}
}
