/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.Tests.Fakes;

namespace TaskOrganizer.Tests.Services;

public class ExportImportServiceTests : IDisposable
{
    private readonly List<string> _fichiersTemporaires = new();

    public void Dispose()
    {
        foreach (var fichier in _fichiersTemporaires)
        {
            if (File.Exists(fichier))
            {
                File.Delete(fichier);
            }
        }
    }

    [Fact]
    public async Task ExporterJsonAsync_EcritUneTacheParLigneDuJson()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache
        {
            Id = 1,
            Titre = "Faire les courses",
            DateEcheance = new DateTime(2026, 6, 1),
            Priorite = PrioriteTache.Haute,
            Statut = StatutTache.ATraiter,
            Categories = { new Categorie { Id = 1, Nom = "Maison" } },
        });
        var service = new ExportImportService(tacheService, new FakeCategorieService());
        var chemin = FichierTemporaire(".json");

        await service.ExporterJsonAsync(chemin);

        var contenu = await File.ReadAllTextAsync(chemin);
        Assert.Contains("Faire les courses", contenu);
        Assert.Contains("Maison", contenu);
    }

    [Fact]
    public async Task ExporterCsvAsync_EcritEnteteEtUneLigneParTache()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache
        {
            Id = 1,
            Titre = "Faire les courses",
            DateEcheance = new DateTime(2026, 6, 1),
            Priorite = PrioriteTache.Haute,
            Statut = StatutTache.ATraiter,
        });
        var service = new ExportImportService(tacheService, new FakeCategorieService());
        var chemin = FichierTemporaire(".csv");

        await service.ExporterCsvAsync(chemin);

        var lignes = await File.ReadAllLinesAsync(chemin);
        Assert.Equal(2, lignes.Length);
        Assert.Equal("Titre;Description;DateEcheance;Priorite;Statut;Categories", lignes[0]);
        Assert.StartsWith("Faire les courses;", lignes[1]);
    }

    [Fact]
    public async Task ExporterCsvAsync_ChampAvecPointVirgule_EstEchappe()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache
        {
            Id = 1,
            Titre = "Acheter; pain et lait",
            DateEcheance = new DateTime(2026, 6, 1),
        });
        var service = new ExportImportService(tacheService, new FakeCategorieService());
        var chemin = FichierTemporaire(".csv");

        await service.ExporterCsvAsync(chemin);

        var lignes = await File.ReadAllLinesAsync(chemin);
        Assert.StartsWith("\"Acheter; pain et lait\";", lignes[1]);
    }

    [Fact]
    public async Task ImporterJsonAsync_FichierExportePrecedemment_RecreeLesTachesEtCategories()
    {
        var tacheServiceExport = new FakeTacheService();
        tacheServiceExport.Taches.Add(new Tache
        {
            Id = 1,
            Titre = "Faire les courses",
            Description = "Lait, pain",
            DateEcheance = new DateTime(2026, 6, 1),
            Priorite = PrioriteTache.Haute,
            Statut = StatutTache.ATraiter,
            Categories = { new Categorie { Id = 1, Nom = "Maison" } },
        });
        var chemin = FichierTemporaire(".json");
        await new ExportImportService(tacheServiceExport, new FakeCategorieService()).ExporterJsonAsync(chemin);

        var tacheServiceImport = new FakeTacheService();
        var service = new ExportImportService(tacheServiceImport, new FakeCategorieService());

        var nombre = await service.ImporterJsonAsync(chemin);

        Assert.Equal(1, nombre);
        var importee = Assert.Single(tacheServiceImport.Taches);
        Assert.Equal("Faire les courses", importee.Titre);
        Assert.Equal("Maison", importee.Categories.Single().Nom);
    }

    private string FichierTemporaire(string extension)
    {
        var chemin = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
        _fichiersTemporaires.Add(chemin);
        return chemin;
    }
}
