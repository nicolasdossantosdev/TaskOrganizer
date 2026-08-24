using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface ICategorieService
{
    Task<IReadOnlyList<Categorie>> ObtenirToutesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Categorie>> ObtenirOuCreerAsync(IReadOnlyList<string> noms, CancellationToken cancellationToken = default);
}
