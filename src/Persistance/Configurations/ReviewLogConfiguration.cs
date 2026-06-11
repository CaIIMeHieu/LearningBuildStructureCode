using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class ReviewLogConfiguration : IEntityTypeConfiguration<Domain.Entities.ReviewLog>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.ReviewLog> builder)
    {
        builder.ToTable("ReviewLogs");
        builder.HasKey(rl => rl.Id);
        builder.Property(rl => rl.CardId).IsRequired();
        builder.Property(rl => rl.Quality)
        .IsRequired()
        .HasMaxLength(10);
        builder.Property(rl => rl.ReviewDate).IsRequired();
        builder.Property(rl => rl.OwnerId).IsRequired();

        builder.HasOne<Domain.Entities.Card>()
               .WithMany()
               .HasForeignKey(rl => rl.CardId)
               .OnDelete(DeleteBehavior.Cascade);        
    }
}
