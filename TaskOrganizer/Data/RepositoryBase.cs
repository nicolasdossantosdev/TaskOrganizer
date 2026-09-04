/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TaskOrganizer.Data;

/// <summary>
/// Fournit à tous les repositories une création de <see cref="AppDbContext"/> par
/// opération (voir <see cref="TacheRepository"/>) ainsi qu'une gestion commune des
/// erreurs SQLite : nouvelle tentative sur verrouillage transitoire (SQLITE_BUSY /
/// SQLITE_LOCKED), et remontée d'une <see cref="PersistanceException"/> au message
/// utilisateur clair pour toute autre erreur d'accès aux données.
/// </summary>
public abstract class RepositoryBase
{
    private const int MaxTentatives = 3;
    private const int SqliteBusy = 5;
    private const int SqliteLocked = 6;

    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    protected RepositoryBase(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    protected async Task<T> ExecuterAsync<T>(Func<AppDbContext, Task<T>> operation, CancellationToken cancellationToken)
    {
        for (var tentative = 1; tentative <= MaxTentatives; tentative++)
        {
            AppDbContext? context = null;
            try
            {
                context = await _contextFactory.CreateDbContextAsync(cancellationToken);
                return await operation(context);
            }
            catch (SqliteException ex) when (EstErreurTransitoire(ex) && tentative < MaxTentatives)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200 * tentative), cancellationToken);
            }
            // EF Core enveloppe les erreurs survenues pendant SaveChangesAsync dans une
            // DbUpdateException : un verrouillage SQLite transitoire (ex. le
            // RappelBackgroundService qui scrute/écrit en tâche de fond toutes les 30s,
            // voir App.xaml.cs) remonte donc ici plutôt que comme SqliteException brute,
            // et doit être retenté comme les autres erreurs transitoires.
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException inner && EstErreurTransitoire(inner) && tentative < MaxTentatives)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200 * tentative), cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new PersistanceException(
                    "Impossible d'enregistrer les données. Vérifiez que le fichier de base de données est accessible.",
                    ex);
            }
            catch (SqliteException ex)
            {
                throw new PersistanceException("Erreur d'accès à la base de données SQLite.", ex);
            }
            finally
            {
                if (context is not null)
                {
                    await context.DisposeAsync();
                }
            }
        }

        throw new PersistanceException("Impossible d'accéder à la base de données après plusieurs tentatives.");
    }

    protected Task ExecuterAsync(Func<AppDbContext, Task> operation, CancellationToken cancellationToken) =>
        ExecuterAsync(
            async context =>
            {
                await operation(context);
                return true;
            },
            cancellationToken);

    private static bool EstErreurTransitoire(SqliteException ex) =>
        ex.SqliteErrorCode is SqliteBusy or SqliteLocked;
}
