using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class CategorieRepository : RepositoryBase, ICategorieRepository
{
    public CategorieRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public Task<IReadOnlyList<Categorie>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ExecuterAsync<IReadOnlyList<Categorie>>(
            async context => await context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Nom)
                .ToListAsync(cancellationToken),
            cancellationToken);

    public Task<IReadOnlyList<Categorie>> GetOrCreateByNomsAsync(
        IReadOnlyList<string> noms,
        CancellationToken cancellationToken = default) =>
        ExecuterAsync<IReadOnlyList<Categorie>>(
            async context =>
            {
                if (noms.Count == 0)
                {
                    return Array.Empty<Categorie>();
                }

                var nomsNormalises = noms
                    .Select(n => n.Trim())
                    .Where(n => n.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var existantes = await context.Categories
                    .Where(c => nomsNormalises.Contains(c.Nom))
                    .ToListAsync(cancellationToken);

                var manquantes = nomsNormalises
                    .Where(nom => !existantes.Any(c => string.Equals(c.Nom, nom, StringComparison.OrdinalIgnoreCase)))
                    .Select(nom => new Categorie { Nom = nom })
                    .ToList();

                if (manquantes.Count > 0)
                {
                    context.Categories.AddRange(manquantes);
                    await context.SaveChangesAsync(cancellationToken);
                }

                return existantes.Concat(manquantes).ToList();
            },
            cancellationToken);
}
