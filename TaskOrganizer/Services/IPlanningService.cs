/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Calcule la période (Jour/Semaine/Mois/Aujourd'hui/À venir) et positionne des
/// tâches déjà chargées sur les jours de cette période. Ne touche pas à la
/// persistance : les tâches lui sont fournies par l'appelant, qui les obtient
/// via <see cref="ITacheService"/> comme partout ailleurs dans l'application.
/// </summary>
public interface IPlanningService
{
    (DateTime Debut, DateTime Fin) ObtenirPeriode(ModePlanning mode, DateTime dateReference);

    IReadOnlyList<JourPlanning> ConstruireJours(IReadOnlyList<Tache> taches, ModePlanning mode, DateTime dateReference);
}
