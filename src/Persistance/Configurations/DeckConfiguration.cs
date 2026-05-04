using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure( EntityTypeBuilder<Deck> builder)
    {
        builder.ToTable("Decks");
        builder.HasKey(d => d.Id);
        // EF hiểu DeckName là một owned entity, nên chúng ta cần cấu hình nó như một owned type
        builder.OwnsOne(d => d.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("DeckName")
                .HasMaxLength(100)
                .IsRequired();
        });
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.HasOne<AppUser>()
               .WithMany()
               .HasForeignKey(d => d.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);
    } 
}
