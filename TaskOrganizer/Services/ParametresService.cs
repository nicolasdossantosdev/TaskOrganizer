/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.IO;
using System.Text.Json;
using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

/// <summary>
/// Persiste les préférences utilisateur (thème, son) dans un fichier JSON,
/// séparément de la base SQLite : ce sont des réglages d'application, pas des
/// données métier. Le chemin est injecté pour rester testable sans toucher au
/// vrai %LOCALAPPDATA%.
/// </summary>
public class ParametresService : IParametresService
{
    private static readonly JsonSerializerOptions OptionsJson = new() { WriteIndented = true };

    private readonly string _cheminFichier;

    public Parametres ParametresActuels { get; private set; } = new();

    public ParametresService(string cheminFichier)
    {
        _cheminFichier = cheminFichier;
    }

    public async Task<Parametres> ChargerAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_cheminFichier))
        {
            ParametresActuels = new Parametres();
            return ParametresActuels;
        }

        try
        {
            await using var flux = File.OpenRead(_cheminFichier);
            var parametres = await JsonSerializer.DeserializeAsync<Parametres>(flux, OptionsJson, cancellationToken);
            ParametresActuels = parametres ?? new Parametres();
        }
        catch (JsonException)
        {
            // Fichier corrompu/illisible : on repart sur des valeurs par défaut
            // plutôt que de bloquer le démarrage de l'application.
            ParametresActuels = new Parametres();
        }

        return ParametresActuels;
    }

    public async Task EnregistrerAsync(Parametres parametres, CancellationToken cancellationToken = default)
    {
        var dossier = Path.GetDirectoryName(_cheminFichier);
        if (!string.IsNullOrEmpty(dossier))
        {
            Directory.CreateDirectory(dossier);
        }

        await using var flux = File.Create(_cheminFichier);
        await JsonSerializer.SerializeAsync(flux, parametres, OptionsJson, cancellationToken);
        ParametresActuels = parametres;
    }
}
