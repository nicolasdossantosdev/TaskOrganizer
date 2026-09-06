/*
 * Task Organizer
 * Copyright (c) 2026 Nicolas Dos Santos
 * Licensed under the MIT License
 */

using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskOrganizer.Data;
using TaskOrganizer.Models;
using TaskOrganizer.Services;

namespace TaskOrganizer.ViewModels;

/// <summary>
/// Fields, validation and save logic shared between the task creation form
/// and the task edit form.
/// </summary>
public abstract partial class TacheFormViewModelBase : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(EnregistrerCommand))]
    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
    private string titre = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(EnregistrerCommand))]
    [Required(ErrorMessage = "La date d'échéance est obligatoire.")]
    private DateTime? dateEcheance;

    [ObservableProperty]
    private PrioriteTache priorite = PrioriteTache.Normale;

    [ObservableProperty]
    private StatutTache statut = StatutTache.ATraiter;

    /// <summary>Comma-separated category names (e.g. "Home, Urgent").</summary>
    [ObservableProperty]
    private string? categoriesTexte;

    [ObservableProperty]
    private string? erreurEnregistrement;

    [ObservableProperty]
    private bool rappelUneHeureAvant;

    [ObservableProperty]
    private bool rappelLaVeille;

    [ObservableProperty]
    private bool rappelUneSemaineAvant;

    public IReadOnlyList<PrioriteTache> PrioritesDisponibles { get; } = Enum.GetValues<PrioriteTache>();

    public IReadOnlyList<StatutTache> StatutsDisponibles { get; } = Enum.GetValues<StatutTache>();

    public event EventHandler<Tache>? TacheEnregistree;

    private readonly IRappelService _rappelService;

    protected TacheFormViewModelBase(IRappelService rappelService)
    {
        _rappelService = rappelService;
        ValidateAllProperties();
    }

    private bool PeutEnregistrer() => !HasErrors;

    [RelayCommand(CanExecute = nameof(PeutEnregistrer))]
    private async Task EnregistrerAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        try
        {
            ErreurEnregistrement = null;
            var nomsCategories = ParseNomsCategories(CategoriesTexte);
            var tache = await PersisterAsync(nomsCategories);
            await _rappelService.DefinirRappelsAsync(tache.Id, tache.DateEcheance, ObtenirOffsetsSelectionnes());
            TacheEnregistree?.Invoke(this, tache);
            ApresEnregistrement();
        }
        catch (PersistanceException ex)
        {
            ErreurEnregistrement = ex.Message;
        }
    }

    /// <summary>Creates or updates the task from the form's properties.</summary>
    protected abstract Task<Tache> PersisterAsync(IReadOnlyList<string> nomsCategories);

    /// <summary>Resets/closes the form after a successful save.</summary>
    protected virtual void ApresEnregistrement()
    {
    }

    protected void RemplirDepuis(Tache tache)
    {
        Titre = tache.Titre;
        Description = tache.Description;
        DateEcheance = tache.DateEcheance;
        Priorite = tache.Priorite;
        Statut = tache.Statut;
        CategoriesTexte = string.Join(", ", tache.Categories.Select(c => c.Nom));

        RappelUneHeureAvant = tache.Rappels.Any(r => r.Offset == OffsetRappel.UneHeureAvant);
        RappelLaVeille = tache.Rappels.Any(r => r.Offset == OffsetRappel.LaVeille);
        RappelUneSemaineAvant = tache.Rappels.Any(r => r.Offset == OffsetRappel.UneSemaineAvant);
    }

    private IReadOnlyList<OffsetRappel> ObtenirOffsetsSelectionnes()
    {
        var offsets = new List<OffsetRappel>();
        if (RappelUneHeureAvant)
        {
            offsets.Add(OffsetRappel.UneHeureAvant);
        }

        if (RappelLaVeille)
        {
            offsets.Add(OffsetRappel.LaVeille);
        }

        if (RappelUneSemaineAvant)
        {
            offsets.Add(OffsetRappel.UneSemaineAvant);
        }

        return offsets;
    }

    private static IReadOnlyList<string> ParseNomsCategories(string? texte) =>
        (texte ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
