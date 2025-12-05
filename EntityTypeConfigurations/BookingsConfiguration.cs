using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceTenants.Models;
using System.Linq;

namespace WorkspaceTenants.Infrastructure{

public class BookingConfiguration : IEntityTypeConfiguration<Bookings>
{
    public void Configure(EntityTypeBuilder<Bookings> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasMaxLength(32).IsRequired();
        b.HasOne<Rooms>().WithMany(r => r.Bookings).HasForeignKey(x => x.RoomId);
        b.HasIndex(x => new { x.TenantId, x.UserId });
    }
}

}