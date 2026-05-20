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
        builder.Property(x => x.Note)
            .HasColumnType("nvarchar(4000)");

        builder.Property(x => x.EaseFactor)
            .HasPrecision(4, 2);
        builder.HasMany(c => c.ReviewLogs)
               .WithOne()
               .HasForeignKey(rl => rl.CardId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Deck>()
               .WithMany()
               .HasForeignKey(c => c.DeckId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(c => new { c.DeckId, c.RecallDate });
    }
}
