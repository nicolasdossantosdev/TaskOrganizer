/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using Microsoft.Extensions.Hosting;

namespace TaskOrganizer.Services;

/// <summary>
/// Periodically polls for due reminders and fires a toast notification for
/// each one. Started/stopped manually from App.xaml.cs (composition root)
/// rather than via the generic .NET Host, to stay simple in a plain WPF app.
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
