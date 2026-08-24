using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data.Configurations;

public class CategorieConfiguration : IEntityTypeConfiguration<Categorie>
{
    public void Configure(EntityTypeBuilder<Categorie> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Nom).IsUnique();
    }
}
