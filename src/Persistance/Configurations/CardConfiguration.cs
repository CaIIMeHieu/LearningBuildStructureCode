using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Domain.Entities.Card>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Card> builder)
    {
        builder.ToTable("Cards");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Question).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Answer).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Note)
            .HasColumnType("nvarchar(4000)");

        builder.Property(c => c.EaseFactor)
            .HasPrecision(4, 2);
        builder.Property(c => c.OwnerId)
            .IsRequired();
        builder.HasOne<Deck>()
               .WithMany()
               .HasForeignKey(c => c.DeckId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(c => new { c.DeckId, c.RecallDate });
        builder.HasIndex(c => new { c.OwnerId, c.Interval });
    }
}
