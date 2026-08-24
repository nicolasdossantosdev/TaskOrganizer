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

    protected override void OnStartup(StartupEventArgs e)
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

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
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
        services.AddSingleton<IDialogService, DialogService>();

        services.AddTransient<CreateTacheViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }
}
