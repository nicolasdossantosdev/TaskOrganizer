using TaskOrganizer.Models;
using TaskOrganizer.Tests.Fakes;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.ViewModels;

public class EditTacheViewModelTests
{
    [Fact]
    public void Constructeur_PreremplitLeFormulaireDepuisLaTache()
    {
        var tache = new Tache
        {
            Id = 42,
            Titre = "Titre existant",
            Description = "Description existante",
            DateEcheance = new DateTime(2026, 6, 1),
            Priorite = PrioriteTache.Haute,
            Statut = StatutTache.EnCours,
            Categories = { new Categorie { Id = 1, Nom = "Maison" } },
            Rappels = { new Rappel { Offset = OffsetRappel.LaVeille, DateHeureRappel = new DateTime(2026, 5, 31) } },
        };

        var viewModel = new EditTacheViewModel(tache, new FakeTacheService(), new FakeCategorieService(), new FakeRappelService());

        Assert.Equal("Titre existant", viewModel.Titre);
        Assert.Equal("Description existante", viewModel.Description);
        Assert.Equal(new DateTime(2026, 6, 1), viewModel.DateEcheance);
        Assert.Equal(PrioriteTache.Haute, viewModel.Priorite);
        Assert.Equal(StatutTache.EnCours, viewModel.Statut);
        Assert.Equal("Maison", viewModel.CategoriesTexte);
        Assert.True(viewModel.RappelLaVeille);
        Assert.False(viewModel.RappelUneHeureAvant);
    }

    [Fact]
    public void Constructeur_FormulairePreremplitValide_EnregistrerCommandEstActivee()
    {
        var tache = new Tache { Id = 1, Titre = "Titre", DateEcheance = DateTime.Today };

        var viewModel = new EditTacheViewModel(tache, new FakeTacheService(), new FakeCategorieService(), new FakeRappelService());

        Assert.True(viewModel.EnregistrerCommand.CanExecute(null));
    }

    [Fact]
    public async Task EnregistrerAsync_ModifieLeTitre_AppelleModifierAsyncAvecLeMemeId()
    {
        var tacheService = new FakeTacheService();
        tacheService.Taches.Add(new Tache { Id = 7, Titre = "Ancien titre", DateEcheance = DateTime.Today });
        var tache = tacheService.Taches[0];

        var viewModel = new EditTacheViewModel(tache, tacheService, new FakeCategorieService(), new FakeRappelService());
        viewModel.Titre = "Nouveau titre";

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.Equal("Nouveau titre", tacheService.Taches.Single(t => t.Id == 7).Titre);
    }

    [Fact]
    public async Task EnregistrerAsync_Reussi_DeclencheEvenementTacheEnregistree()
    {
        var tacheService = new FakeTacheService();
        var tache = new Tache { Id = 3, Titre = "T", DateEcheance = DateTime.Today };
        tacheService.Taches.Add(tache);

        var viewModel = new EditTacheViewModel(tache, tacheService, new FakeCategorieService(), new FakeRappelService());

        Tache? recue = null;
        viewModel.TacheEnregistree += (_, t) => recue = t;

        await viewModel.EnregistrerCommand.ExecuteAsync(null);

        Assert.NotNull(recue);
        Assert.Equal(3, recue!.Id);
    }
}
