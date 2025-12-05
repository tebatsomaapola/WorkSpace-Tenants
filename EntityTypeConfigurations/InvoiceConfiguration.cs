using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceTenants.Models;

namespace WorkspaceTenants{

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoices>
{
    public void Configure(EntityTypeBuilder<Invoices> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasMaxLength(32).IsRequired();
        b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        b.HasIndex(x => new { x.TenantId, x.UserId });
    }
}
}
