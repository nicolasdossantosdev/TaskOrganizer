using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class CreateTacheViewModelTests
{
    [Fact]
    public void Constructeur_FormulaireVide_EnregistrerCommandEstDesactivee()
    {
        var viewModel = new CreateTacheViewModel(new FakeTacheService());

        Assert.False(viewModel.EnregistrerCommand.CanExecute(null));
    }

    [Fact]
    public void Titre_ViseVide_ProduitUneErreurDeValidation()
    {
        var viewModel = new CreateTacheViewModel(new FakeTacheService())
        {
            Titre = "Temporaire",
        };

        viewModel.Titre = string.Empty;

        Assert.True(viewModel.GetErrors(nameof(viewModel.Titre)).Cast<object>().Any());
    }

    [Fact]
    public void ChampsObligatoiresRenseignes_EnregistrerCommandEstActivee()
    {
        var viewModel = new CreateTacheViewModel(new FakeTacheService())
        {
            Titre = "Faire les courses",
            DateEcheance = DateTime.Today.AddDays(1),
        };

        Assert.True(viewModel.EnregistrerCommand.CanExecute(null));
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireValide_AppelleLeServiceEtReinitialiseLeFormulaire()
    {
        var service = new FakeTacheService();
        var viewModel = new CreateTacheViewModel(service)
        {
            Titre = "Faire les courses",
            Description = "Lait, oeufs",
            DateEcheance = DateTime.Today.AddDays(1),
            Categorie = "Maison",
        };

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.Single(service.TachesCreees);
        Assert.Equal("Faire les courses", service.TachesCreees[0].Titre);
        Assert.Equal(string.Empty, viewModel.Titre);
        Assert.Null(viewModel.DateEcheance);
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireValide_DeclencheEvenementTacheCreee()
    {
        var viewModel = new CreateTacheViewModel(new FakeTacheService())
        {
            Titre = "Faire les courses",
            DateEcheance = DateTime.Today.AddDays(1),
        };

        Tache? tacheRecue = null;
        viewModel.TacheCreee += (_, tache) => tacheRecue = tache;

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.NotNull(tacheRecue);
        Assert.Equal("Faire les courses", tacheRecue!.Titre);
    }

    [Fact]
    public async Task EnregistrerAsync_FormulaireInvalide_NAppellePasLeService()
    {
        var service = new FakeTacheService();
        var viewModel = new CreateTacheViewModel(service);

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.Empty(service.TachesCreees);
    }

    private sealed class FakeTacheService : ITacheService
    {
        public List<Tache> TachesCreees { get; } = new();

        public Task<IReadOnlyList<Tache>> ObtenirToutesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Tache>>(TachesCreees);

        public Task<Tache> CreerAsync(Tache tache, CancellationToken cancellationToken = default)
        {
            tache.Id = TachesCreees.Count + 1;
            TachesCreees.Add(tache);
            return Task.FromResult(tache);
        }

        public Task SupprimerAsync(int id, CancellationToken cancellationToken = default)
        {
            TachesCreees.RemoveAll(t => t.Id == id);
            return Task.CompletedTask;
        }
    }
}
