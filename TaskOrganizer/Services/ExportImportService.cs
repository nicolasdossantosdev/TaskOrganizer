using System.IO;
using System.Text;
using System.Text.Json;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Réutilise ITacheService/ICategorieService pour lire/écrire les tâches
/// (aucun accès direct à Data) : l'export/import est une orchestration
/// applicative, pas une nouvelle source de vérité pour la persistance.
/// </summary>
public class ExportImportService : IExportImportService
{
    private static readonly JsonSerializerOptions OptionsJson = new() { WriteIndented = true };

    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;

    public ExportImportService(ITacheService tacheService, ICategorieService categorieService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
    }

    public async Task ExporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        var dtos = await ConstruireDtosAsync(cancellationToken);

        await using var flux = File.Create(cheminFichier);
        await JsonSerializer.SerializeAsync(flux, dtos, OptionsJson, cancellationToken);
    }

    public async Task ExporterCsvAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        var dtos = await ConstruireDtosAsync(cancellationToken);

        await using var writer = new StreamWriter(cheminFichier, append: false, Encoding.UTF8);
        await writer.WriteLineAsync("Titre;Description;DateEcheance;Priorite;Statut;Categories");

        foreach (var dto in dtos)
        {
            var colonnes = new[]
            {
                EchapperCsv(dto.Titre),
                EchapperCsv(dto.Description ?? string.Empty),
                dto.DateEcheance.ToString("yyyy-MM-dd"),
                dto.Priorite.ToString(),
                dto.Statut.ToString(),
                EchapperCsv(string.Join(", ", dto.Categories)),
            };
            await writer.WriteLineAsync(string.Join(';', colonnes));
        }
    }

    public async Task<int> ImporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        await using var flux = File.OpenRead(cheminFichier);
        var dtos = await JsonSerializer.DeserializeAsync<List<TacheExportDto>>(flux, OptionsJson, cancellationToken)
            ?? new List<TacheExportDto>();

        foreach (var dto in dtos)
        {
            var categories = await _categorieService.ObtenirOuCreerAsync(dto.Categories, cancellationToken);

            var tache = new Tache
            {
                Titre = dto.Titre,
                Description = dto.Description,
                DateEcheance = dto.DateEcheance,
                Priorite = dto.Priorite,
                Statut = dto.Statut,
            };
            foreach (var categorie in categories)
            {
                tache.Categories.Add(categorie);
            }

            await _tacheService.CreerAsync(tache, cancellationToken);
        }

        return dtos.Count;
    }

    private async Task<List<TacheExportDto>> ConstruireDtosAsync(CancellationToken cancellationToken)
    {
        var taches = await _tacheService.ObtenirToutesAsync(cancellationToken);

        return taches
            .Select(t => new TacheExportDto
            {
                Titre = t.Titre,
                Description = t.Description,
                DateEcheance = t.DateEcheance,
                Priorite = t.Priorite,
                Statut = t.Statut,
                Categories = t.Categories.Select(c => c.Nom).ToList(),
            })
            .ToList();
    }

    private static string EchapperCsv(string valeur)
    {
        if (valeur.IndexOfAny(new[] { ';', '"', '\n', '\r' }) < 0)
        {
            return valeur;
        }

        return "\"" + valeur.Replace("\"", "\"\"") + "\"";
    }
}
