using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITacheService _tacheService;

    public CreateTacheViewModel CreateTacheViewModel { get; }

    public ObservableCollection<Tache> Taches { get; } = new();

    public MainViewModel(ITacheService tacheService, CreateTacheViewModel createTacheViewModel)
    {
        _tacheService = tacheService;
        CreateTacheViewModel = createTacheViewModel;
        CreateTacheViewModel.TacheCreee += (_, tache) => Taches.Insert(0, tache);

        ChargerCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var taches = await _tacheService.ObtenirToutesAsync();

        Taches.Clear();
        foreach (var tache in taches)
        {
            Taches.Add(tache);
        }
    }
}
