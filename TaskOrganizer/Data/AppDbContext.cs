/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

/// <summary>
/// EF Core database context for the SQLite-backed persistence layer.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tache> Taches => Set<Tache>();

    public DbSet<Categorie> Categories => Set<Categorie>();

    public DbSet<Rappel> Rappels => Set<Rappel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
