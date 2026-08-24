using System.ComponentModel.DataAnnotations;

namespace TaskOrganizer.Models;

public class Tache
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateTime DateEcheance { get; set; }

    public PrioriteTache Priorite { get; set; } = PrioriteTache.Normale;

    public StatutTache Statut { get; set; } = StatutTache.ATraiter;

    [MaxLength(100)]
    public string? Categorie { get; set; }
}
