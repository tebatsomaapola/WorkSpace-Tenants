using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkspaceTenants.Models;
using System.Collections.Generic;


namespace WorkspaceTenants{

public class RoomConfiguration : IEntityTypeConfiguration<Rooms>
{
    public void Configure(EntityTypeBuilder<Rooms> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(128).IsRequired();
        b.Property(x => x.HourlyRate).HasColumnType("decimal(18,2)");
        b.HasOne<Workspace>().WithMany(w => w.Rooms).HasForeignKey(x => x.WorkspaceId);
    }
}
}
