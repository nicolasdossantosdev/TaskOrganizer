using System.Windows;
using TaskOrganizer.Models;
using TaskOrganizer.Services;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

public class DialogService : IDialogService
{
    private readonly ITacheService _tacheService;
    private readonly ICategorieService _categorieService;
    private readonly IRappelService _rappelService;

    public DialogService(ITacheService tacheService, ICategorieService categorieService, IRappelService rappelService)
    {
        _tacheService = tacheService;
        _categorieService = categorieService;
        _rappelService = rappelService;
    }

    public bool OuvrirFenetreEdition(Tache tache)
    {
        var viewModel = new EditTacheViewModel(tache, _tacheService, _categorieService, _rappelService);
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

    public void AfficherInformation(string message)
    {
        MessageBox.Show(
            Application.Current.MainWindow,
            message,
            "TaskOrganizer",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public string? ChoisirFichierExport(string nomFichierParDefaut, string filtre)
    {
        var dialogue = new Microsoft.Win32.SaveFileDialog
        {
            FileName = nomFichierParDefaut,
            Filter = filtre,
        };
        return dialogue.ShowDialog(Application.Current.MainWindow) == true ? dialogue.FileName : null;
    }

    public string? ChoisirFichierImport(string filtre)
    {
        var dialogue = new Microsoft.Win32.OpenFileDialog
        {
            Filter = filtre,
        };
        return dialogue.ShowDialog(Application.Current.MainWindow) == true ? dialogue.FileName : null;
    }
}
