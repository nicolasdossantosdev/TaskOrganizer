/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Abstraction over WPF dialogs (edit window, confirmations, error messages)
/// so ViewModels stay testable without depending directly on WPF types.
/// Implemented in Views (see DialogService) and injected via DI.
/// </summary>
public interface IDialogService
{
    /// <summary>Opens the task edit window. Returns true if the user saved.</summary>
    bool OuvrirFenetreEdition(Tache tache);

    bool ConfirmerSuppression(Tache tache);

    void AfficherErreur(string message);

    void AfficherInformation(string message);

    /// <summary>"Save as" dialog. Returns the chosen path, or null if cancelled.</summary>
    string? ChoisirFichierExport(string nomFichierParDefaut, string filtre);

    /// <summary>"Open" dialog. Returns the chosen path, or null if cancelled.</summary>
    string? ChoisirFichierImport(string filtre);
}
