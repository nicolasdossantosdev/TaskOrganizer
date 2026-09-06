/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>Displays reminder notifications to the user.</summary>
public interface INotificationService
{
    void AfficherRappel(Rappel rappel);

    void AfficherResumeRappelsManques(IReadOnlyList<Rappel> rappelsManques);
}
