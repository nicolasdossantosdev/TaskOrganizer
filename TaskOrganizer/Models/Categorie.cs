/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.ComponentModel.DataAnnotations;

namespace TaskOrganizer.Models;

public class Categorie
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    public ICollection<Tache> Taches { get; set; } = new List<Tache>();
}
