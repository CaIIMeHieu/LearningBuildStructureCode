using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        // Shared PK với AspNetUsers — không auto-generate
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.TimeZoneId)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("UTC");

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        // Streak là VO → owned type → map thành columns trong UserProfiles table
        builder.OwnsOne(u => u.Streak, streakBuilder =>
        {
            streakBuilder.Property(s => s.Current)
                .HasColumnName("StreakCurrent")
                .HasDefaultValue(0)
                .IsRequired();

            streakBuilder.Property(s => s.Longest)
                .HasColumnName("StreakLongest")
                .HasDefaultValue(0)
                .IsRequired();

            streakBuilder.Property(s => s.LastReviewedDate)
                .HasColumnName("LastReviewedDate")
                .IsRequired(false);

            builder.Property(u => u.TotalReviews)
                    .HasDefaultValue(0)
                    .IsRequired();
        });

        // UserBadge là Entity → bảng riêng → HasMany
        builder.HasMany(u => u.Badges)
            .WithOne()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK 1-1 với AspNetUsers
        builder.HasOne<Domain.Entities.AppUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
