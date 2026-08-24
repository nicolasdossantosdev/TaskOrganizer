using TaskOrganizer.Models;

namespace TaskOrganizer.Services;

public interface IParametresService
{
    /// <summary>Derniers paramètres chargés/enregistrés (valeurs par défaut tant que ChargerAsync n'a pas été appelé).</summary>
    Parametres ParametresActuels { get; }

    Task<Parametres> ChargerAsync(CancellationToken cancellationToken = default);

    Task EnregistrerAsync(Parametres parametres, CancellationToken cancellationToken = default);
}
