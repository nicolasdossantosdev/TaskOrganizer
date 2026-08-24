using TaskOrganizer.Models;
using TaskOrganizer.Tests.Fakes;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class ParametresViewModelTests
{
    [Fact]
    public void Constructeur_PreremplitDepuisLesParametresActuelsSansReenregistrer()
    {
        var parametresService = new FakeParametresService();
        parametresService.ParametresActuels.Theme = ThemeApplication.Sombre;
        parametresService.ParametresActuels.SonNotificationsActif = false;
        var themeService = new FakeThemeService();

        var viewModel = CreerViewModel(parametresService, themeService);

        Assert.Equal(ThemeApplication.Sombre, viewModel.ThemeSelectionne);
        Assert.False(viewModel.SonNotificationsActif);
        // Le thème est réappliqué au démarrage (pour que l'UI reflète le thème
        // sauvegardé), mais aucune écriture disque ne doit avoir lieu tant que
        // l'utilisateur n'a rien changé.
        Assert.Empty(parametresService.ParametresEnregistres);
    }

    [Fact]
    public void ChangerTheme_AppliqueLeThemeEtEnregistreLesParametres()
    {
        var parametresService = new FakeParametresService();
        var themeService = new FakeThemeService();
        var viewModel = CreerViewModel(parametresService, themeService);

        viewModel.ThemeSelectionne = ThemeApplication.Sombre;

        Assert.Contains(ThemeApplication.Sombre, themeService.ThemesAppliques);
        Assert.Equal(ThemeApplication.Sombre, parametresService.ParametresEnregistres.Single().Theme);
    }

    [Fact]
    public void ChangerSonNotificationsActif_EnregistreLesParametres()
    {
        var parametresService = new FakeParametresService();
        var viewModel = CreerViewModel(parametresService);

        viewModel.SonNotificationsActif = false;

        Assert.False(parametresService.ParametresEnregistres.Single().SonNotificationsActif);
    }

    [Fact]
    public async Task ExporterJsonAsync_AucunCheminChoisi_NAppellePasLeService()
    {
        var exportImportService = new FakeExportImportService();
        var dialogService = new FakeDialogService { CheminFichierExportChoisi = null };
        var viewModel = CreerViewModel(exportImportService: exportImportService, dialogService: dialogService);

        await viewModel.ExporterJsonCommand.ExecuteAsync(null);

        Assert.Empty(exportImportService.CheminsExportesJson);
    }

    [Fact]
    public async Task ExporterJsonAsync_CheminChoisi_ExporteEtAfficheUneConfirmation()
    {
        var exportImportService = new FakeExportImportService();
        var dialogService = new FakeDialogService { CheminFichierExportChoisi = @"C:\taches.json" };
        var viewModel = CreerViewModel(exportImportService: exportImportService, dialogService: dialogService);

        await viewModel.ExporterJsonCommand.ExecuteAsync(null);

        Assert.Equal(new[] { @"C:\taches.json" }, exportImportService.CheminsExportesJson);
        Assert.Single(dialogService.InformationsAffichees);
    }

    [Fact]
    public async Task ExporterCsvAsync_CheminChoisi_ExporteEnCsv()
    {
        var exportImportService = new FakeExportImportService();
        var dialogService = new FakeDialogService { CheminFichierExportChoisi = @"C:\taches.csv" };
        var viewModel = CreerViewModel(exportImportService: exportImportService, dialogService: dialogService);

        await viewModel.ExporterCsvCommand.ExecuteAsync(null);

        Assert.Equal(new[] { @"C:\taches.csv" }, exportImportService.CheminsExportesCsv);
    }

    [Fact]
    public async Task ImporterJsonAsync_Reussi_DeclencheDonneesImportees()
    {
        var exportImportService = new FakeExportImportService { NombreAImporter = 3 };
        var dialogService = new FakeDialogService { CheminFichierImportChoisi = @"C:\taches.json" };
        var viewModel = CreerViewModel(exportImportService: exportImportService, dialogService: dialogService);

        var declenche = false;
        viewModel.DonneesImportees += (_, _) => declenche = true;

        await viewModel.ImporterJsonCommand.ExecuteAsync(null);

        Assert.True(declenche);
        Assert.Single(dialogService.InformationsAffichees);
    }

    [Fact]
    public async Task ImporterJsonAsync_ServiceLeveUneException_AfficheUneErreurSansDeclencherLevenement()
    {
        var exportImportService = new FakeExportImportService { ExceptionSurImport = new IOException("Fichier verrouillé") };
        var dialogService = new FakeDialogService { CheminFichierImportChoisi = @"C:\taches.json" };
        var viewModel = CreerViewModel(exportImportService: exportImportService, dialogService: dialogService);

        var declenche = false;
        viewModel.DonneesImportees += (_, _) => declenche = true;

        await viewModel.ImporterJsonCommand.ExecuteAsync(null);

        Assert.False(declenche);
        Assert.Single(dialogService.ErreursAffichees);
    }

    private static ParametresViewModel CreerViewModel(
        FakeParametresService? parametresService = null,
        FakeThemeService? themeService = null,
        FakeExportImportService? exportImportService = null,
        FakeDialogService? dialogService = null)
        => new(
            parametresService ?? new FakeParametresService(),
            themeService ?? new FakeThemeService(),
            exportImportService ?? new FakeExportImportService(),
            dialogService ?? new FakeDialogService());
}
