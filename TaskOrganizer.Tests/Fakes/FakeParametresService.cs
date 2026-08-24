using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeParametresService : IParametresService
{
    public Parametres ParametresActuels { get; private set; } = new();

    public List<Parametres> ParametresEnregistres { get; } = new();

    public Task<Parametres> ChargerAsync(CancellationToken cancellationToken = default) => Task.FromResult(ParametresActuels);

    public Task EnregistrerAsync(Parametres parametres, CancellationToken cancellationToken = default)
    {
        ParametresActuels = parametres;
        ParametresEnregistres.Add(parametres);
        return Task.CompletedTask;
    }
}
