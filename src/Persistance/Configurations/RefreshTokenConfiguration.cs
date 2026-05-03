using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<Domain.Entities.RefreshToken>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DeviceId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<Domain.Entities.AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.DeviceId, x.Token });
    }
}
