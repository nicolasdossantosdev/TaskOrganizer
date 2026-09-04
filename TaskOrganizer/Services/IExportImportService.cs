/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Services;

public interface IExportImportService
{
    Task ExporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default);

    Task ExporterCsvAsync(string cheminFichier, CancellationToken cancellationToken = default);

    /// <summary>Importe les tâches du fichier JSON (format produit par <see cref="ExporterJsonAsync"/>) et retourne le nombre de tâches créées.</summary>
    Task<int> ImporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default);
}
