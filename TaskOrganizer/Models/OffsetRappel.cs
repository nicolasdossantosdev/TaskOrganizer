/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Models;

/// <summary>Offset of a reminder relative to the task's due date.</summary>
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
