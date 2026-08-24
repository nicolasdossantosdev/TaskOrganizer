using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data.Configurations;

public class RappelConfiguration : IEntityTypeConfiguration<Rappel>
{
    public void Configure(EntityTypeBuilder<Rappel> builder)
    {
        builder.ToTable("Rappels");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Offset)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.DateHeureRappel)
            .IsRequired();

        builder.HasOne(r => r.Tache)
            .WithMany(t => t.Rappels)
            .HasForeignKey(r => r.TacheId)
            .OnDelete(DeleteBehavior.Cascade);

        // Requête de scrutation du service d'arrière-plan : rappels non
        // déclenchés dont la date/heure est passée.
        builder.HasIndex(r => new { r.Declenche, r.DateHeureRappel });
    }
}
