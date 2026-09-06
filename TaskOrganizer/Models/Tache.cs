/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskOrganizer.Models;

/// <summary>Task entity — the core domain object.</summary>
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

    public ICollection<Categorie> Categories { get; set; } = new List<Categorie>();

    public ICollection<Rappel> Rappels { get; set; } = new List<Rappel>();

    [NotMapped]
    public string CategoriesAffichees => string.Join(", ", Categories.Select(c => c.Nom));
}
