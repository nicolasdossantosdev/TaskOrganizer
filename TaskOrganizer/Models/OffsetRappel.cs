/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Models;

/// <summary>Décalage d'un rappel par rapport à l'échéance de la tâche.</summary>
public enum OffsetRappel
{
    UneHeureAvant,
    LaVeille,
    UneSemaineAvant,
}

public static class OffsetRappelExtensions
{
    public static TimeSpan VersDelai(this OffsetRappel offset) => offset switch
    {
        OffsetRappel.UneHeureAvant => TimeSpan.FromHours(1),
        OffsetRappel.LaVeille => TimeSpan.FromDays(1),
        OffsetRappel.UneSemaineAvant => TimeSpan.FromDays(7),
        _ => throw new ArgumentOutOfRangeException(nameof(offset), offset, null),
    };
}
