using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Format d'échange stable pour l'export/import, découplé du schéma EF Core
/// (évite les cycles Tache&lt;-&gt;Categorie et les colonnes internes type Id).
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
