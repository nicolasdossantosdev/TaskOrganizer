/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Tests.Fakes;

internal sealed class FakeDialogService : IDialogService
{
    public bool ConfirmerSuppressionReponse { get; set; } = true;

    public bool OuvrirFenetreEditionReponse { get; set; } = true;

    public string? CheminFichierExportChoisi { get; set; }

    public string? CheminFichierImportChoisi { get; set; }

    public List<string> ErreursAffichees { get; } = new();

    public List<string> InformationsAffichees { get; } = new();

    public bool OuvrirFenetreEdition(Tache tache) => OuvrirFenetreEditionReponse;

    public bool ConfirmerSuppression(Tache tache) => ConfirmerSuppressionReponse;

    public void AfficherErreur(string message) => ErreursAffichees.Add(message);

    public void AfficherInformation(string message) => InformationsAffichees.Add(message);

    public string? ChoisirFichierExport(string nomFichierParDefaut, string filtre) => CheminFichierExportChoisi;

    public string? ChoisirFichierImport(string filtre) => CheminFichierImportChoisi;
}
