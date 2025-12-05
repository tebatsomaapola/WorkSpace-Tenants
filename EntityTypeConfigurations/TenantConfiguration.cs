using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceTenants.Models;

namespace WorkspaceTenants{

public class TenantConfiguration : IEntityTypeConfiguration<Tenants>
{
    public void Configure(EntityTypeBuilder<Tenants> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(128).IsRequired();
    }
}
}
