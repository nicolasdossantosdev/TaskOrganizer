/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Computes the period (Day/Week/Month/Today/Upcoming) and places already-loaded
/// tasks onto the days of that period. Does not touch persistence: tasks are
/// supplied by the caller, which obtains them via <see cref="ITacheService"/>
/// like everywhere else in the application.
/// </summary>
public interface IPlanningService
{
    (DateTime Debut, DateTime Fin) ObtenirPeriode(ModePlanning mode, DateTime dateReference);

    IReadOnlyList<JourPlanning> ConstruireJours(IReadOnlyList<Tache> taches, ModePlanning mode, DateTime dateReference);
}
