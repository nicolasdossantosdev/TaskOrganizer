/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

namespace TaskOrganizer.Services;

/// <summary>Export/import of tasks to/from JSON and CSV.</summary>
public interface IExportImportService
{
    Task ExporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default);

    Task ExporterCsvAsync(string cheminFichier, CancellationToken cancellationToken = default);

    /// <summary>Imports tasks from the JSON file (format produced by <see cref="ExporterJsonAsync"/>) and returns the number of tasks created.</summary>
    Task<int> ImporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default);
}
