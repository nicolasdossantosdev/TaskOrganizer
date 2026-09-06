/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Pure calculation implementation of <see cref="IPlanningService"/>.</summary>
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

    /// <summary>ISO week (Monday -> Sunday).</summary>
    private static (DateTime Debut, DateTime Fin) ObtenirSemaine(DateTime date)
    {
        var decalage = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var lundi = date.AddDays(-decalage);
        return (lundi, lundi.AddDays(6));
    }

    /// <summary>Full month, extended to whole weeks (for a gap-free grid display).</summary>
    private static (DateTime Debut, DateTime Fin) ObtenirMoisEtendu(DateTime date)
    {
        var premierDuMois = new DateTime(date.Year, date.Month, 1);
        var dernierDuMois = premierDuMois.AddMonths(1).AddDays(-1);

        var (debutSemaine, _) = ObtenirSemaine(premierDuMois);
        var (_, finSemaine) = ObtenirSemaine(dernierDuMois);

        return (debutSemaine, finSemaine);
    }
}
