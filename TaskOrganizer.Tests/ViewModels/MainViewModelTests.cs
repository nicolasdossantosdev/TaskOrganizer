using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.Tests.Fakes;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class MainViewModelTests
{
    private static MainViewModel CreerViewModel(
        FakeTacheService? tacheService = null,
        FakeDialogService? dialogService = null)
    {
        tacheService ??= new FakeTacheService();
        dialogService ??= new FakeDialogService();
        var createTacheViewModel = new CreateTacheViewModel(tacheService, new FakeCategorieService(), new FakeRappelService());
        var planningViewModel = new PlanningViewModel(tacheService, new PlanningService(), dialogService);
        var parametresViewModel = new ParametresViewModel(
            new FakeParametresService(),
            new FakeThemeService(),
            new FakeExportImportService(),
            dialogService);
        return new MainViewModel(tacheService, dialogService, createTacheViewModel, planningViewModel, parametresViewModel);
    }

    [Fact]
    public void Constructeur_ChargeLesTachesExistantes()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today });
        tacheService.Taches.Add(new Tache { Id = 2, Titre = "B", DateEcheance = DateTime.Today });

        var viewModel = CreerViewModel(tacheService);

        Assert.Equal(2, viewModel.Taches.Count);
    }

    [Fact]
    public void TexteRecherche_FiltreParTitre()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 1, Titre = "Faire les courses", DateEcheance = DateTime.Today });
        tacheService.Taches.Add(new Tache { Id = 2, Titre = "Payer les factures", DateEcheance = DateTime.Today });
        var viewModel = CreerViewModel(tacheService);

        viewModel.TexteRecherche = "courses";

        var visibles = viewModel.TachesAffichees.Cast<Tache>().ToList();
        Assert.Single(visibles);
        Assert.Equal("Faire les courses", visibles[0].Titre);
    }

    [Fact]
    public void TexteRecherche_FiltreParDescription()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 1, Titre = "A", Description = "Acheter du lait", DateEcheance = DateTime.Today });
        tacheService.Taches.Add(new Tache { Id = 2, Titre = "B", Description = "Rien à voir", DateEcheance = DateTime.Today });
        var viewModel = CreerViewModel(tacheService);

        viewModel.TexteRecherche = "lait";

        Assert.Single(viewModel.TachesAffichees.Cast<Tache>());
    }

    [Fact]
    public void TriActuel_ParPriorite_TrieLaListeAffichee()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 1, Titre = "Haute", Priorite = PrioriteTache.Haute, DateEcheance = DateTime.Today });
        tacheService.Taches.Add(new Tache { Id = 2, Titre = "Basse", Priorite = PrioriteTache.Basse, DateEcheance = DateTime.Today });
        var viewModel = CreerViewModel(tacheService);

        viewModel.TriActuel = CritereTri.Priorite;

        var ordre = viewModel.TachesAffichees.Cast<Tache>().Select(t => t.Titre).ToList();
        Assert.Equal(new[] { "Basse", "Haute" }, ordre);
    }

    [Fact]
    public async Task SupprimerAsync_ConfirmationAcceptee_SupprimeLaTache()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var dialogService = new FakeDialogService { ConfirmerSuppressionReponse = true };
        var viewModel = CreerViewModel(tacheService, dialogService);

        await viewModel.SupprimerCommand.ExecuteAsync(tache);

        Assert.Empty(viewModel.Taches);
        Assert.Empty(tacheService.Taches);
    }

    [Fact]
    public async Task SupprimerAsync_ConfirmationRefusee_NeSupprimeRien()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var dialogService = new FakeDialogService { ConfirmerSuppressionReponse = false };
        var viewModel = CreerViewModel(tacheService, dialogService);

        await viewModel.SupprimerCommand.ExecuteAsync(tache);

        Assert.Single(viewModel.Taches);
        Assert.Single(tacheService.Taches);
    }

    [Fact]
    public async Task ChangerStatutAsync_PersisteLaTache()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", Statut = StatutTache.ATraiter, DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var viewModel = CreerViewModel(tacheService);
        tache.Statut = StatutTache.Terminee;

        await viewModel.ChangerStatutCommand.ExecuteAsync(tache);

        Assert.Equal(StatutTache.Terminee, tacheService.Taches.Single(t => t.Id == 1).Statut);
    }

    [Fact]
    public void Modifier_OuvertureAcceptee_RechargeLesTaches()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var dialogService = new FakeDialogService { OuvrirFenetreEditionReponse = true };
        var viewModel = CreerViewModel(tacheService, dialogService);

        viewModel.ModifierCommand.Execute(tache);

        Assert.Single(viewModel.Taches);
    }
}
