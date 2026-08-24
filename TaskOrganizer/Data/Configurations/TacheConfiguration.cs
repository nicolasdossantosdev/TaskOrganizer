using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data.Configurations;

public class TacheConfiguration : IEntityTypeConfiguration<Tache>
{
    public void Configure(EntityTypeBuilder<Tache> builder)
    {
        builder.ToTable("Taches");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Titre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.DateEcheance)
            .IsRequired();

        builder.Property(t => t.Priorite)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Statut)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasMany(t => t.Categories)
            .WithMany(c => c.Taches)
            .UsingEntity(j => j.ToTable("TacheCategories"));
    }
}
