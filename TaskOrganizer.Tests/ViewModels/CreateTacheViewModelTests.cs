using TaskOrganizer.Models;
using TaskOrganizer.Tests.Fakes;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class CreateTacheViewModelTests
{
    private static CreateTacheViewModel CreerViewModel(
        FakeTacheService? tacheService = null,
        FakeCategorieService? categorieService = null,
        FakeRappelService? rappelService = null) =>
        new(tacheService ?? new FakeTacheService(), categorieService ?? new FakeCategorieService(), rappelService ?? new FakeRappelService());

    [Fact]
    public void Constructeur_FormulaireVide_EnregistrerCommandEstDesactivee()
    {
        var viewModel = CreerViewModel();

        Assert.False(viewModel.EnregistrerCommand.CanExecute(null));
    }

    [Fact]
    public void Titre_ViseVide_ProduitUneErreurDeValidation()
    {
        var viewModel = CreerViewModel();
        viewModel.Titre = "Temporaire";

        viewModel.Titre = string.Empty;

        Assert.True(viewModel.GetErrors(nameof(viewModel.Titre)).Cast<object>().Any());
    }

    [Fact]
    public void ChampsObligatoiresRenseignes_EnregistrerCommandEstActivee()
    {
        var viewModel = CreerViewModel();
        viewModel.Titre = "Faire les courses";
        viewModel.DateEcheance = DateTime.Today.AddDays(1);

        Assert.True(viewModel.EnregistrerCommand.CanExecute(null));
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireValide_AppelleLeServiceEtReinitialiseLeFormulaire()
    {
        var service = new FakeTacheService();
        var viewModel = CreerViewModel(service);
        viewModel.Titre = "Faire les courses";
        viewModel.Description = "Lait, oeufs";
        viewModel.DateEcheance = DateTime.Today.AddDays(1);
        viewModel.CategoriesTexte = "Maison";

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.Single(service.Taches);
        Assert.Equal("Faire les courses", service.Taches[0].Titre);
        Assert.Equal(string.Empty, viewModel.Titre);
        Assert.Null(viewModel.DateEcheance);
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireValide_DeclencheEvenementTacheCreee()
    {
        var viewModel = CreerViewModel();
        viewModel.Titre = "Faire les courses";
        viewModel.DateEcheance = DateTime.Today.AddDays(1);

        Tache? tacheRecue = null;
        viewModel.TacheEnregistree += (_, tache) => tacheRecue = tache;

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.NotNull(tacheRecue);
        Assert.Equal("Faire les courses", tacheRecue!.Titre);
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireInvalide_NAppellePasLeService()
    {
        var service = new FakeTacheService();
        var viewModel = CreerViewModel(service);

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.Empty(service.Taches);
    }

    [Fact]
    public async Task EnregistrerAsync_AvecRappelsCoches_DefinitLesRappelsSurLaTache()
    {
        var tacheService = new FakeTacheService();
        var rappelService = new FakeRappelService();
        var viewModel = CreerViewModel(tacheService, rappelService: rappelService);
        viewModel.Titre = "Faire les courses";
        viewModel.DateEcheance = DateTime.Today.AddDays(1);
        viewModel.RappelLaVeille = true;
        viewModel.RappelUneHeureAvant = true;

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        var tacheId = tacheService.Taches[0].Id;
        var offsets = rappelService.RappelsParTache[tacheId];
        Assert.Equal(2, offsets.Count);
        Assert.Contains(OffsetRappel.LaVeille, offsets);
        Assert.Contains(OffsetRappel.UneHeureAvant, offsets);
    }
}
