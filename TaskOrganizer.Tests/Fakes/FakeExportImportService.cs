using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeExportImportService : IExportImportService
{
    public List<string> CheminsExportesJson { get; } = new();

    public List<string> CheminsExportesCsv { get; } = new();

    public int NombreAImporter { get; set; }

    public Exception? ExceptionSurImport { get; set; }

    public Task ExporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        CheminsExportesJson.Add(cheminFichier);
        return Task.CompletedTask;
    }

    public Task ExporterCsvAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        CheminsExportesCsv.Add(cheminFichier);
        return Task.CompletedTask;
    }

    public Task<int> ImporterJsonAsync(string cheminFichier, CancellationToken cancellationToken = default)
    {
        if (ExceptionSurImport is not null)
        {
            throw ExceptionSurImport;
        }

        return Task.FromResult(NombreAImporter);
    }
}
