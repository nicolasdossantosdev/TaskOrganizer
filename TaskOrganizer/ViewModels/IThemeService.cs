/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Applique le thème clair/sombre/système à l'application. Défini ici (comme
/// <see cref="IDialogService"/>) mais implémenté dans Views car ça manipule du
/// WPF (<c>Application.ThemeMode</c>) ; garde <see cref="ParametresViewModel"/>
/// testable sans dépendre de WPF.
/// </summary>
public interface IThemeService
{
    void Appliquer(ThemeApplication theme);
}
