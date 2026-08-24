using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public class PlanningService : IPlanningService
{
    public (DateTime Debut, DateTime Fin) ObtenirPeriode(ModePlanning mode, DateTime dateReference)
    {
        var date = dateReference.Date;

        return mode switch
        {
            ModePlanning.Jour => (date, date),
            ModePlanning.Aujourdhui => (DateTime.Today, DateTime.Today),
            ModePlanning.AVenir => (DateTime.Today, DateTime.Today.AddDays(6)),
            ModePlanning.Semaine => ObtenirSemaine(date),
            ModePlanning.Mois => ObtenirMoisEtendu(date),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    public IReadOnlyList<JourPlanning> ConstruireJours(
        IReadOnlyList<Tache> taches,
        ModePlanning mode,
        DateTime dateReference)
    {
        var (debut, fin) = ObtenirPeriode(mode, dateReference);
        var moisPrincipal = mode == ModePlanning.Mois ? (int?)dateReference.Month : null;

        var jours = new List<JourPlanning>();
        for (var date = debut; date <= fin; date = date.AddDays(1))
        {
            var tachesDuJour = taches
                .Where(t => t.DateEcheance.Date == date)
                .OrderByDescending(t => t.Priorite)
                .ThenBy(t => t.Titre, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var estDansLaPeriodePrincipale = moisPrincipal is null || date.Month == moisPrincipal;
            jours.Add(new JourPlanning(date, tachesDuJour, estDansLaPeriodePrincipale));
        }

        return jours;
    }

    /// <summary>Semaine ISO (lundi -> dimanche).</summary>
    private static (DateTime Debut, DateTime Fin) ObtenirSemaine(DateTime date)
    {
        var decalage = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var lundi = date.AddDays(-decalage);
        return (lundi, lundi.AddDays(6));
    }

    /// <summary>Mois complet, étendu aux semaines entières (pour un affichage en grille sans trou).</summary>
    private static (DateTime Debut, DateTime Fin) ObtenirMoisEtendu(DateTime date)
    {
        var premierDuMois = new DateTime(date.Year, date.Month, 1);
        var dernierDuMois = premierDuMois.AddMonths(1).AddDays(-1);

        var (debutSemaine, _) = ObtenirSemaine(premierDuMois);
        var (_, finSemaine) = ObtenirSemaine(dernierDuMois);

        return (debutSemaine, finSemaine);
    }
}
