using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Services;

public class ParametresServiceTests : IDisposable
{
    private readonly string _cheminFichier = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

    public void Dispose()
    {
        if (File.Exists(_cheminFichier))
        {
            File.Delete(_cheminFichier);
        }
    }

    [Fact]
    public async Task ChargerAsync_FichierInexistant_RetourneLesValeursParDefaut()
    {
        var service = new ParametresService(_cheminFichier);

        var parametres = await service.ChargerAsync();

        Assert.Equal(ThemeApplication.Systeme, parametres.Theme);
        Assert.True(parametres.SonNotificationsActif);
    }

    [Fact]
    public async Task EnregistrerAsyncPuisChargerAsync_RetourneLesMemesValeurs()
    {
        var service = new ParametresService(_cheminFichier);
        await service.EnregistrerAsync(new Parametres { Theme = ThemeApplication.Sombre, SonNotificationsActif = false });

        var relu = await new ParametresService(_cheminFichier).ChargerAsync();

        Assert.Equal(ThemeApplication.Sombre, relu.Theme);
        Assert.False(relu.SonNotificationsActif);
    }

    [Fact]
    public async Task EnregistrerAsync_MetAJourParametresActuels()
    {
        var service = new ParametresService(_cheminFichier);

        await service.EnregistrerAsync(new Parametres { Theme = ThemeApplication.Clair });

        Assert.Equal(ThemeApplication.Clair, service.ParametresActuels.Theme);
    }

    [Fact]
    public async Task ChargerAsync_FichierCorrompu_RetourneLesValeursParDefautSansLever()
    {
        await File.WriteAllTextAsync(_cheminFichier, "{ ceci n'est pas du json valide");
        var service = new ParametresService(_cheminFichier);

        var parametres = await service.ChargerAsync();

        Assert.Equal(ThemeApplication.Systeme, parametres.Theme);
    }
}
