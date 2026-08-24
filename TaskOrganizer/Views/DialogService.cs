using System.Windows;
using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

public class DialogService : IDialogService
{
    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;

    public DialogService(ITacheService tacheService, ICategorieService categorieService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
    }

    public bool OuvrirFenetreEdition(Tache tache)
    {
        var viewModel = new EditTacheViewModel(tache, _tacheService, _categorieService);
        var fenetre = new EditTacheWindow(viewModel)
        {
            Owner = Application.Current.MainWindow,
        };
        return fenetre.ShowDialog() == true;
    }

    public bool ConfirmerSuppression(Tache tache)
    {
        var resultat = MessageBox.Show(
            Application.Current.MainWindow,
            $"Supprimer la tâche « {tache.Titre} » ?",
            "Confirmer la suppression",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        return resultat == MessageBoxResult.Yes;
    }

    public void AfficherErreur(string message)
    {
        MessageBox.Show(
            Application.Current.MainWindow,
            message,
            "Erreur",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
