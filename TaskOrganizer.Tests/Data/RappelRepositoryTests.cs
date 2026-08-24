using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Data;
using TaskOrganizer.Models;

namespace TaskOrganizer.Tests.Data;

public class RappelRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RappelRepository _rappelRepository;
    private readonly TacheRepository _tacheRepository;

    public RappelRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        var factory = new TestDbContextFactory(options);
        _rappelRepository = new RappelRepository(factory);
        _tacheRepository = new TacheRepository(factory);
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task RemplacerPourTacheAsync_PremiereFois_CreeLesRappels()
    {
        var tache = await _tacheRepository.AddAsync(new Tache { Titre = "T", DateEcheance = DateTime.Today });
        var rappels = new[]
        {
            new Rappel { Offset = OffsetRappel.LaVeille, DateHeureRappel = DateTime.Today.AddDays(-1) },
            new Rappel { Offset = OffsetRappel.UneHeureAvant, DateHeureRappel = DateTime.Today.AddHours(-1) },
        };

        await _rappelRepository.RemplacerPourTacheAsync(tache.Id, rappels);

        var recuperee = await _tacheRepository.GetByIdAsync(tache.Id);
        Assert.Equal(2, recuperee!.Rappels.Count);
    }

    [Fact]
    public async Task RemplacerPourTacheAsync_AppelRepete_RemplaceLesAnciensRappels()
    {
        var tache = await _tacheRepository.AddAsync(new Tache { Titre = "T", DateEcheance = DateTime.Today });
        await _rappelRepository.RemplacerPourTacheAsync(
            tache.Id,
            new[] { new Rappel { Offset = OffsetRappel.LaVeille, DateHeureRappel = DateTime.Today.AddDays(-1) } });

        await _rappelRepository.RemplacerPourTacheAsync(
            tache.Id,
            new[] { new Rappel { Offset = OffsetRappel.UneSemaineAvant, DateHeureRappel = DateTime.Today.AddDays(-7) } });

        var recuperee = await _tacheRepository.GetByIdAsync(tache.Id);
        Assert.Single(recuperee!.Rappels);
        Assert.Equal(OffsetRappel.UneSemaineAvant, recuperee.Rappels.Single().Offset);
    }

    [Fact]
    public async Task ObtenirEtMarquerRappelsDusAsync_RappelDansLePasse_EstRetourneEtMarqueDeclenche()
    {
        var tache = await _tacheRepository.AddAsync(new Tache { Titre = "T", DateEcheance = DateTime.Today });
        await _rappelRepository.RemplacerPourTacheAsync(
            tache.Id,
            new[] { new Rappel { Offset = OffsetRappel.LaVeille, DateHeureRappel = DateTime.Now.AddMinutes(-5) } });

        var dus = await _rappelRepository.ObtenirEtMarquerRappelsDusAsync(DateTime.Now);

        Assert.Single(dus);
        Assert.Equal(tache.Id, dus[0].TacheId);
        Assert.NotNull(dus[0].Tache);

        var deuxiemeAppel = await _rappelRepository.ObtenirEtMarquerRappelsDusAsync(DateTime.Now);
        Assert.Empty(deuxiemeAppel);
    }

    [Fact]
    public async Task ObtenirEtMarquerRappelsDusAsync_RappelDansLeFutur_NEstPasRetourne()
    {
        var tache = await _tacheRepository.AddAsync(new Tache { Titre = "T", DateEcheance = DateTime.Today.AddDays(10) });
        await _rappelRepository.RemplacerPourTacheAsync(
            tache.Id,
            new[] { new Rappel { Offset = OffsetRappel.LaVeille, DateHeureRappel = DateTime.Now.AddDays(9) } });

        var dus = await _rappelRepository.ObtenirEtMarquerRappelsDusAsync(DateTime.Now);

        Assert.Empty(dus);
    }
}
