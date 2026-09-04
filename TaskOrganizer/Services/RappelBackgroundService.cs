/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.Extensions.Hosting;

namespace TaskOrganizer.Services;

/// <summary>
/// Scrute périodiquement les rappels dus et déclenche une notification toast
/// pour chacun. Démarré/arrêté manuellement depuis App.xaml.cs (composition
/// root) plutôt que via le générique .NET Host, pour rester simple dans une
/// application WPF classique.
/// </summary>
public class RappelBackgroundService : BackgroundService
{
    private static readonly TimeSpan IntervallePolling = TimeSpan.FromSeconds(30);

    private readonly IRappelService _rappelService;
    private readonly INotificationService _notificationService;

    public RappelBackgroundService(IRappelService rappelService, INotificationService notificationService)
    {
        _rappelService = rappelService;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(IntervallePolling);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var rappelsDus = await _rappelService.ObtenirEtMarquerRappelsDusAsync(DateTime.Now, stoppingToken);
            foreach (var rappel in rappelsDus)
            {
                _notificationService.AfficherRappel(rappel);
            }
        }
    }
}
