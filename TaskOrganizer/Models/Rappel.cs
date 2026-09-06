/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Models;

/// <summary>Scheduled reminder for a task.</summary>
public class Rappel
{
    public int Id { get; set; }

    public int TacheId { get; set; }

    public Tache? Tache { get; set; }

    public OffsetRappel Offset { get; set; }

    /// <summary>
    /// Date/heure absolue à laquelle le rappel doit se déclencher, calculée à
    /// partir de <see cref="Tache.DateEcheance"/> et <see cref="Offset"/> au
    /// moment de la création/modification de la tâche. Stockée telle quelle
    /// (plutôt que recalculée à chaque scrutation) pour que le service
    /// d'arrière-plan puisse simplement comparer à DateTime.Now.
    /// </summary>
    public DateTime DateHeureRappel { get; set; }

    public bool Declenche { get; set; }
}
