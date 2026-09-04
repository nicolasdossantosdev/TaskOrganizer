/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.Tests.Services;

public class PlanningServiceTests
{
    private readonly PlanningService _service = new();

    [Fact]
    public void ObtenirPeriode_Jour_DebutEtFinSontLaMemeDate()
    {
        var date = new DateTime(2026, 3, 17);

        var (debut, fin) = _service.ObtenirPeriode(ModePlanning.Jour, date);

        Assert.Equal(date, debut);
        Assert.Equal(date, fin);
    }

    [Fact]
    public void ObtenirPeriode_Aujourdhui_IgnoreLaDateReferenceEtUtiliseAujourdhui()
    {
        var dateReferenceLointaine = new DateTime(2030, 1, 1);

        var (debut, fin) = _service.ObtenirPeriode(ModePlanning.Aujourdhui, dateReferenceLointaine);

        Assert.Equal(DateTime.Today, debut);
        Assert.Equal(DateTime.Today, fin);
    }

    [Fact]
    public void ObtenirPeriode_AVenir_CouvreAujourdhuiPlusSixJours()
    {
        var (debut, fin) = _service.ObtenirPeriode(ModePlanning.AVenir, new DateTime(2030, 1, 1));

        Assert.Equal(DateTime.Today, debut);
        Assert.Equal(DateTime.Today.AddDays(6), fin);
        Assert.Equal(7, (fin - debut).Days + 1);
    }

    [Fact]
    public void ObtenirPeriode_Semaine_CouvreDuLundiAuDimancheEtContientLaDate()
    {
        var mercredi = new DateTime(2026, 3, 18); // un mercredi

        var (debut, fin) = _service.ObtenirPeriode(ModePlanning.Semaine, mercredi);

        Assert.Equal(DayOfWeek.Monday, debut.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, fin.DayOfWeek);
        Assert.Equal(6, (fin - debut).Days);
        Assert.InRange(mercredi, debut, fin);
    }

    [Fact]
    public void ObtenirPeriode_Mois_EtendAuxSemainesCompletesEtCouvreToutLeMois()
    {
        var dateDansLeMois = new DateTime(2026, 1, 14);
        var premierDuMois = new DateTime(2026, 1, 1);
        var dernierDuMois = new DateTime(2026, 1, 31);

        var (debut, fin) = _service.ObtenirPeriode(ModePlanning.Mois, dateDansLeMois);

        Assert.Equal(DayOfWeek.Monday, debut.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, fin.DayOfWeek);
        Assert.True(debut <= premierDuMois);
        Assert.True(fin >= dernierDuMois);
        Assert.Equal(0, ((fin - debut).Days + 1) % 7);
    }

    [Fact]
    public void ConstruireJours_Semaine_RegroupeChaqueTacheSurSaDateDecheance()
    {
        var lundi = new DateTime(2026, 3, 16);
        var taches = new List<Tache>
        {
            new() { Titre = "Lundi", DateEcheance = lundi },
            new() { Titre = "Mercredi", DateEcheance = lundi.AddDays(2) },
        };

        var jours = _service.ConstruireJours(taches, ModePlanning.Semaine, lundi);

        Assert.Equal(7, jours.Count);
        Assert.Single(jours[0].Taches);
        Assert.Equal("Lundi", jours[0].Taches[0].Titre);
        Assert.Single(jours[2].Taches);
        Assert.Equal("Mercredi", jours[2].Taches[0].Titre);
        Assert.Empty(jours[1].Taches);
    }

    [Fact]
    public void ConstruireJours_Mois_MarqueLesJoursHorsMoisCommeHorsPeriodePrincipale()
    {
        var jours = _service.ConstruireJours(Array.Empty<Tache>(), ModePlanning.Mois, new DateTime(2026, 1, 14));

        Assert.Contains(jours, j => !j.EstDansLaPeriodePrincipale);
        Assert.All(jours.Where(j => j.Date.Month == 1 && j.Date.Year == 2026), j => Assert.True(j.EstDansLaPeriodePrincipale));
        Assert.All(jours.Where(j => j.Date.Month != 1), j => Assert.False(j.EstDansLaPeriodePrincipale));
    }

    [Fact]
    public void ConstruireJours_Aujourdhui_NeContientQueLaTacheDuJour()
    {
        var taches = new List<Tache>
        {
            new() { Titre = "Aujourd'hui", DateEcheance = DateTime.Today },
            new() { Titre = "Demain", DateEcheance = DateTime.Today.AddDays(1) },
        };

        var jours = _service.ConstruireJours(taches, ModePlanning.Aujourdhui, DateTime.Today);

        var jour = Assert.Single(jours);
        Assert.True(jour.EstAujourdhui);
        Assert.Single(jour.Taches);
        Assert.Equal("Aujourd'hui", jour.Taches[0].Titre);
    }

    [Fact]
    public void ConstruireJours_AVenir_CouvreSeptJoursDepuisAujourdhui()
    {
        var taches = new List<Tache>
        {
            new() { Titre = "Aujourd'hui", DateEcheance = DateTime.Today },
            new() { Titre = "Dans 6 jours", DateEcheance = DateTime.Today.AddDays(6) },
            new() { Titre = "Trop tard", DateEcheance = DateTime.Today.AddDays(7) },
        };

        var jours = _service.ConstruireJours(taches, ModePlanning.AVenir, DateTime.Today);

        Assert.Equal(7, jours.Count);
        Assert.Contains(jours, j => j.Taches.Any(t => t.Titre == "Aujourd'hui"));
        Assert.Contains(jours, j => j.Taches.Any(t => t.Titre == "Dans 6 jours"));
        Assert.DoesNotContain(jours, j => j.Taches.Any(t => t.Titre == "Trop tard"));
    }

    [Fact]
    public void ConstruireJours_PlusieursTachesLeMemeJour_TrieesParPrioriteDecroissante()
    {
        var date = new DateTime(2026, 3, 16);
        var taches = new List<Tache>
        {
            new() { Titre = "Basse", DateEcheance = date, Priorite = PrioriteTache.Basse },
            new() { Titre = "Haute", DateEcheance = date, Priorite = PrioriteTache.Haute },
            new() { Titre = "Normale", DateEcheance = date, Priorite = PrioriteTache.Normale },
        };

        var jours = _service.ConstruireJours(taches, ModePlanning.Jour, date);

        var ordre = jours[0].Taches.Select(t => t.Titre).ToList();
        Assert.Equal(new[] { "Haute", "Normale", "Basse" }, ordre);
    }
}
