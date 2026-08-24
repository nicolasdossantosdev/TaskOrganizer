using Microsoft.Toolkit.Uwp.Notifications;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Notifications toast Windows natives (Centre de notifications), via Microsoft.Toolkit.Uwp.Notifications.</summary>
public class ToastNotificationService : INotificationService
{
    public void AfficherRappel(Rappel rappel)
    {
        var titreTache = rappel.Tache?.Titre ?? "Tâche";
        var echeance = rappel.Tache is not null
            ? rappel.Tache.DateEcheance.ToString("dd/MM/yyyy")
            : null;

        var builder = new ToastContentBuilder()
            .AddText("Rappel de tâche")
            .AddText(titreTache);

        if (echeance is not null)
        {
            builder.AddText($"Échéance : {echeance}");
        }

        builder.Show();
    }

    public void AfficherResumeRappelsManques(IReadOnlyList<Rappel> rappelsManques)
    {
        if (rappelsManques.Count == 0)
        {
            return;
        }

        new ToastContentBuilder()
            .AddText("Rappels manqués")
            .AddText($"{rappelsManques.Count} rappel(s) n'ont pas pu être affichés pendant que l'application était fermée.")
            .Show();
    }
}
