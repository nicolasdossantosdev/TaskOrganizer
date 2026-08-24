using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.Tests.Fakes;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class PlanningViewModelTests
{
    private static PlanningViewModel CreerViewModel(
        FakeTacheService? tacheService = null,
        FakeDialogService? dialogService = null) =>
        new(tacheService ?? new FakeTacheService(), new PlanningService(), dialogService ?? new FakeDialogService());

    [Fact]
    public void Constructeur_ChargeLesTachesEtConstruitLesJoursDeLaSemaine()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today });

        var viewModel = CreerViewModel(tacheService);

        Assert.Equal(7, viewModel.JoursAffiches.Count);
        Assert.Contains(viewModel.JoursAffiches, j => j.Taches.Any(t => t.Titre == "A"));
    }

    [Fact]
    public async Task ReplanifierAsync_NouvelleDateDifferente_MetAJourEtPersisteLaTache()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var viewModel = CreerViewModel(tacheService);
        var nouvelleDate = DateTime.Today.AddDays(3);

        await viewModel.ReplanifierCommand.ExecuteAsync((tache, nouvelleDate));

        Assert.Equal(nouvelleDate.Date, tache.DateEcheance);
        Assert.Equal(nouvelleDate.Date, tacheService.Taches.Single(t => t.Id == 1).DateEcheance);
    }

    [Fact]
    public async Task ReplanifierAsync_MemeDate_NeModifieRienEtNAppellePasLeService()
    {
        var tacheService = new FakeTacheService();
        var date = DateTime.Today;
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = date };
        tacheService.Taches.Add(tache);
        var viewModel = CreerViewModel(tacheService);

        await viewModel.ReplanifierCommand.ExecuteAsync((tache, date));

        Assert.Equal(date, tache.DateEcheance);
    }

    [Fact]
    public async Task ReplanifierAsync_EchecPersistance_RevertLaDateEtAfficheUneErreur()
    {
        var tacheService = new FakeTacheService { ExceptionSurModifier = new PersistanceException("Erreur DB") };
        var dateInitiale = DateTime.Today;
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = dateInitiale };
        tacheService.Taches.Add(tache);
        var dialogService = new FakeDialogService();
        var viewModel = CreerViewModel(tacheService, dialogService);

        await viewModel.ReplanifierCommand.ExecuteAsync((tache, DateTime.Today.AddDays(5)));

        Assert.Equal(dateInitiale, tache.DateEcheance);
        Assert.Single(dialogService.ErreursAffichees);
    }

    [Fact]
    public async Task ReplanifierAsync_Succes_DeclencheEvenementTacheReplanifiee()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 1, Titre = "A", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);
        var viewModel = CreerViewModel(tacheService);

        var declenche = false;
        viewModel.TacheReplanifiee += (_, _) => declenche = true;

        await viewModel.ReplanifierCommand.ExecuteAsync((tache, DateTime.Today.AddDays(1)));

        Assert.True(declenche);
    }

    [Fact]
    public void PeriodeSuivante_ModeSemaine_AvanceLaDateReferenceDeSeptJours()
    {
        var viewModel = CreerViewModel();
        viewModel.ModeActuel = ModePlanning.Semaine;
        var dateInitiale = viewModel.DateReference;

        viewModel.PeriodeSuivanteCommand.Execute(null);

        Assert.Equal(dateInitiale.AddDays(7), viewModel.DateReference);
    }

    [Fact]
    public void PeriodePrecedente_ModeJour_ReculeLaDateReferenceDUnJour()
    {
        var viewModel = CreerViewModel();
        viewModel.ModeActuel = ModePlanning.Jour;
        var dateInitiale = viewModel.DateReference;

        viewModel.PeriodePrecedenteCommand.Execute(null);

        Assert.Equal(dateInitiale.AddDays(-1), viewModel.DateReference);
    }

    [Fact]
    public void AllerAAujourdhui_ReinitialiseLaDateReferenceSurAujourdhui()
    {
        var viewModel = CreerViewModel();
        viewModel.DateReference = DateTime.Today.AddMonths(2);

        viewModel.AllerAAujourdhuiCommand.Execute(null);

        Assert.Equal(DateTime.Today, viewModel.DateReference);
    }

    [Theory]
    [InlineData(ModePlanning.Jour, 1)]
    [InlineData(ModePlanning.Aujourdhui, 1)]
    [InlineData(ModePlanning.Semaine, 7)]
    [InlineData(ModePlanning.Mois, 7)]
    [InlineData(ModePlanning.AVenir, 7)]
    public void ColonnesAffichage_DependDuMode(ModePlanning mode, int colonnesAttendues)
    {
        var viewModel = CreerViewModel();

        viewModel.ModeActuel = mode;

        Assert.Equal(colonnesAttendues, viewModel.ColonnesAffichage);
    }
}
