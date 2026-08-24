using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface INotificationService
{
    void AfficherRappel(Rappel rappel);

    void AfficherResumeRappelsManques(IReadOnlyList<Rappel> rappelsManques);
}
