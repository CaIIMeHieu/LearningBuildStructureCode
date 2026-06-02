using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Configurations;

public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserBadge> builder)
    {
        builder.ToTable("UserBadges");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserId)
            .IsRequired();
        builder.Property(b => b.BadgeType)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(b => b.EarnedAt)
            .IsRequired();
        // Invariant: không earn badge 2 lần
        builder.HasIndex(b => new { b.UserId, b.BadgeType })
            .IsUnique();
        // Query badges của 1 user
        builder.HasIndex(b => b.UserId);
    }
}
