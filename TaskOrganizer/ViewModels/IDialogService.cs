using TaskOrganizer.Models;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Abstraction des boîtes de dialogue WPF (fenêtre d'édition, confirmations,
/// messages d'erreur) afin que les ViewModels restent testables sans dépendre
/// directement de types WPF. Implémentée dans Views (voir DialogService) et
/// injectée via DI.
/// </summary>
public interface IDialogService
{
    /// <summary>Ouvre la fenêtre d'édition d'une tâche. Retourne true si l'utilisateur a enregistré.</summary>
    bool OuvrirFenetreEdition(Tache tache);

    bool ConfirmerSuppression(Tache tache);

    void AfficherErreur(string message);

    void AfficherInformation(string message);

    /// <summary>Boîte de dialogue "Enregistrer sous". Retourne le chemin choisi, ou null si annulé.</summary>
    string? ChoisirFichierExport(string nomFichierParDefaut, string filtre);

    /// <summary>Boîte de dialogue "Ouvrir". Retourne le chemin choisi, ou null si annulé.</summary>
    string? ChoisirFichierImport(string filtre);
}
