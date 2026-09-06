/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Stable exchange format for export/import, decoupled from the EF Core schema
/// (avoids Tache&lt;-&gt;Categorie cycles and internal columns like Id).
/// </summary>
public class TacheExportDto
{
    public string Titre { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DateEcheance { get; set; }

    public PrioriteTache Priorite { get; set; }

    public StatutTache Statut { get; set; }

    public List<string> Categories { get; set; } = new();
}
