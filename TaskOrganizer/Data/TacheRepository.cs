/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Models;

namespace TaskOrganizer.Data;

public class TacheRepository : RepositoryBase, ITacheRepository
{
    public TacheRepository(IDbContextFactory<AppDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public Task<IReadOnlyList<Tache>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ExecuterAsync<IReadOnlyList<Tache>>(
            async context => await context.Taches
                .AsNoTracking()
                .Include(t => t.Categories)
                .Include(t => t.Rappels)
                .OrderBy(t => t.DateEcheance)
                .ToListAsync(cancellationToken),
            cancellationToken);

    public Task<Tache?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context => await context.Taches
                .Include(t => t.Categories)
                .Include(t => t.Rappels)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken),
            cancellationToken);

    public Task<Tache> AddAsync(Tache tache, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                // Les catégories viennent de ICategorieService.ObtenirOuCreerAsync et sont
                // déjà persistées (via un autre DbContext, donc détachées ici) : il faut
                // remplacer les instances détachées par les entités rattachées (Unchanged)
                // avant d'ajouter la tâche, sinon EF Core les considère comme de nouvelles
                // entités du graphe et tente de les réinsérer, ce qui viole la contrainte de
                // clé primaire (même logique que UpdateAsync).
                var categoriesRattachees = RattacherCategoriesExistantes(context, tache.Categories);
                tache.Categories.Clear();
                foreach (var categorie in categoriesRattachees)
                {
                    tache.Categories.Add(categorie);
                }

                context.Taches.Add(tache);
                await context.SaveChangesAsync(cancellationToken);
                return tache;
            },
            cancellationToken);

    public Task UpdateAsync(Tache tache, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                var existante = await context.Taches
                    .Include(t => t.Categories)
                    .FirstOrDefaultAsync(t => t.Id == tache.Id, cancellationToken);
                if (existante is null)
                {
                    return;
                }

                context.Entry(existante).CurrentValues.SetValues(tache);

                // Ne retire/rattache que ce qui change réellement plutôt qu'un Clear() suivi
                // d'un ré-Add() des mêmes catégories : sur une relation many-to-many à
                // navigation implicite (skip navigation), rejouer Clear()+Add() de la même
                // entité dans le même SaveChangesAsync ne régénère pas fiablement la ligne
                // de jointure supprimée par Clear().
                var nouveauxIds = tache.Categories.Select(c => c.Id).ToHashSet();
                foreach (var categorie in existante.Categories.Where(c => !nouveauxIds.Contains(c.Id)).ToList())
                {
                    existante.Categories.Remove(categorie);
                }

                var idsExistants = existante.Categories.Select(c => c.Id).ToHashSet();
                var categoriesAAjouter = tache.Categories.Where(c => !idsExistants.Contains(c.Id));
                foreach (var categorie in RattacherCategoriesExistantes(context, categoriesAAjouter))
                {
                    existante.Categories.Add(categorie);
                }

                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);

    /// <summary>
    /// Rattache des catégories déjà persistées (donc détachées, venant d'un autre
    /// DbContext ou d'une requête AsNoTracking) comme Unchanged. On attache un stub
    /// (Id + Nom seulement) plutôt que l'entité reçue telle quelle : Attach parcourt
    /// tout le graphe atteignable, et une Categorie chargée via Tache.Categories peut
    /// porter une collection Taches "fixup" par EF (même sans .Include(c => c.Taches))
    /// contenant la Tache déjà suivie dans ce contexte — l'attacher provoquerait un
    /// conflit d'identité ("cannot be tracked because another instance ... is already
    /// being tracked") et faisait planter l'application (voir Sprint 4).
    /// </summary>
    private static IReadOnlyList<Categorie> RattacherCategoriesExistantes(AppDbContext context, IEnumerable<Categorie> categories) =>
        categories
            .Select(categorie => context.Categories.Local.FirstOrDefault(c => c.Id == categorie.Id)
                ?? context.Categories.Attach(new Categorie { Id = categorie.Id, Nom = categorie.Nom }).Entity)
            .ToList();

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        ExecuterAsync(
            async context =>
            {
                var tache = await context.Taches.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
                if (tache is null)
                {
                    return;
                }

                context.Taches.Remove(tache);
                await context.SaveChangesAsync(cancellationToken);
            },
            cancellationToken);
}
