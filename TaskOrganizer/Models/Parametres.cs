/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Models;

/// <summary>User-persisted application settings.</summary>
public class Parametres
{
    public ThemeApplication Theme { get; set; } = ThemeApplication.Systeme;

    public bool SonNotificationsActif { get; set; } = true;
}
