using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskOrganizer.Data;
using TaskOrganizer.Services;
using TaskOrganizer.ViewModels;
using TaskOrganizer.Views;

namespace TaskOrganizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private RappelBackgroundService? _rappelBackgroundService;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using (var dbContext = contextFactory.CreateDbContext())
        {
            dbContext.Database.Migrate();
        }

        // Rappels dus pendant que l'application était fermée : on les "draine"
        // en un résumé avant de démarrer la scrutation en direct, pour ne pas
        // les redéclencher individuellement en tant que toasts.
        var rappelService = _serviceProvider.GetRequiredService<IRappelService>();
        var rappelsManques = await rappelService.ObtenirEtMarquerRappelsDusAsync(DateTime.Now);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        if (rappelsManques.Count > 0)
        {
            var fenetreRappelsManques = new RappelsManquesWindow(rappelsManques) { Owner = mainWindow };
            fenetreRappelsManques.ShowDialog();
        }

        _rappelBackgroundService = _serviceProvider.GetRequiredService<RappelBackgroundService>();
        await _rappelBackgroundService.StartAsync(CancellationToken.None);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_rappelBackgroundService is not null)
        {
            await _rappelBackgroundService.StopAsync(CancellationToken.None);
        }

        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TaskOrganizer");
        Directory.CreateDirectory(appDataFolder);
        var dbPath = Path.Combine(appDataFolder, "taskorganizer.db");

        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddSingleton<ITacheRepository, TacheRepository>();
        services.AddSingleton<ITacheService, TacheService>();
        services.AddSingleton<ICategorieRepository, CategorieRepository>();
        services.AddSingleton<ICategorieService, CategorieService>();
        services.AddSingleton<IRappelRepository, RappelRepository>();
        services.AddSingleton<IRappelService, RappelService>();
        services.AddSingleton<INotificationService, ToastNotificationService>();
        services.AddSingleton<IPlanningService, PlanningService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<RappelBackgroundService>();

        services.AddTransient<CreateTacheViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }
}
