/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Data;

/// <summary>
/// Data access error meant to be shown to the user (message already in
/// plain language, without EF Core/SQLite technical detail).
/// </summary>
public class PersistanceException : Exception
{
    public PersistanceException(string message)
        : base(message)
    {
    }

    public PersistanceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
