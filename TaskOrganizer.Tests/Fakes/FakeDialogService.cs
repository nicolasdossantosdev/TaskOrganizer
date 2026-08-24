using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeDialogService : IDialogService
{
    public bool ConfirmerSuppressionReponse { get; set; } = true;

    public bool OuvrirFenetreEditionReponse { get; set; } = true;

    public List<string> ErreursAffichees { get; } = new();

    public bool OuvrirFenetreEdition(Tache tache) => OuvrirFenetreEditionReponse;

    public bool ConfirmerSuppression(Tache tache) => ConfirmerSuppressionReponse;

    public void AfficherErreur(string message) => ErreursAffichees.Add(message);
}
