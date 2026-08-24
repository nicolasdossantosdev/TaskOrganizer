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
}
