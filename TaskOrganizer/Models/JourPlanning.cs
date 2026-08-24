namespace TaskOrganizer.Models;

/// <summary>Projection d'un jour du planning : sa date et les tâches dont l'échéance tombe ce jour-là.</summary>
public class JourPlanning
{
    public JourPlanning(DateTime date, IReadOnlyList<Tache> taches, bool estDansLaPeriodePrincipale = true)
    {
        Date = date.Date;
        Taches = taches;
        EstDansLaPeriodePrincipale = estDansLaPeriodePrincipale;
    }

    public DateTime Date { get; }

    public IReadOnlyList<Tache> Taches { get; }

    /// <summary>
    /// False pour les jours de "padding" affichés en vue Mois (fin du mois
    /// précédent / début du mois suivant, pour compléter des semaines entières).
    /// </summary>
    public bool EstDansLaPeriodePrincipale { get; }

    public bool EstAujourdhui => Date == DateTime.Today;
}
