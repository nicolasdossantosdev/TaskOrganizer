/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Models;

/// <summary>Projection of one planning day: its date and the tasks due that day.</summary>
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
    /// False for "padding" days shown in Month view (end of previous month /
    /// start of next month, to fill out whole weeks).
    /// </summary>
    public bool EstDansLaPeriodePrincipale { get; }

    public bool EstAujourdhui => Date == DateTime.Today;
}
