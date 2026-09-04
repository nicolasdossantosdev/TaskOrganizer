/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface INotificationService
{
    void AfficherRappel(Rappel rappel);

    void AfficherResumeRappelsManques(IReadOnlyList<Rappel> rappelsManques);
}
