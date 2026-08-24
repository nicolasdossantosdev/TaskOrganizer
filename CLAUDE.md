# TaskOrganizer

## Contexte
Application desktop Windows en C# / WPF permettant d'organiser des tâches :
créer/planifier des tâches, les visualiser sur un calendrier, et recevoir des
rappels (notifications). Projet personnel destiné à être présenté dans un
portfolio freelance (Malt, Upwork, Freelancer.com) — le code doit donc être
propre, testé et bien documenté, pas juste fonctionnel.

## Périmètre V1 (MVP)
- CRUD complet des tâches (titre, description, échéance, priorité, statut, catégorie)
- Vue liste avec filtres/tri/recherche
- Vue planning (calendrier jour/semaine/mois) avec replanification par drag & drop
- Rappels programmés avec notifications toast Windows natives (pas de sync mobile en V1)
- Persistance locale en SQLite via EF Core
- Thème clair/sombre

Hors périmètre V1 (V2 potentielle) : notifications sur téléphone (Pushover /
Telegram bot / Firebase Cloud Messaging), synchronisation cloud, multi-utilisateur.

## Stack technique imposée
- .NET (dernière LTS) + WPF
- Pattern MVVM avec CommunityToolkit.Mvvm
- Injection de dépendances : Microsoft.Extensions.DependencyInjection
- Accès aux données : EF Core + SQLite, via un Repository pattern
- Tests unitaires : xUnit (ou NUnit) pour ViewModels et Services

## Architecture / structure de dossiers

```
TaskOrganizer.slnx
CLAUDE.md
README.md
src/
  TaskOrganizer.App/         WPF, exécutable, composition root (DI + App.xaml)
  TaskOrganizer.Core/        Entités, enums, interfaces (repositories/services). Aucune dépendance externe.
  TaskOrganizer.Data/        EF Core : DbContext, Migrations, implémentations des repositories.
  TaskOrganizer.Services/    Logique applicative (orchestration, futurs rappels/notifications).
  TaskOrganizer.ViewModels/  ViewModels MVVM (CommunityToolkit.Mvvm).
  TaskOrganizer.Views/       Fenêtres/UserControls XAML. Pas de logique métier dans le code-behind.
tests/
  TaskOrganizer.Tests/       Tests xUnit (Repositories, Services, ViewModels).
```

Sens des dépendances (un projet ne référence que ceux en dessous de lui) :

```
App
 ├─> Views ─────────> ViewModels ─┐
 ├─> Services ────────────────────┼─> Core
 └─> Data ────────────────────────┘
```

- `Core` ne dépend de rien d'autre dans la solution (entités POCO + interfaces).
- `Data` dépend de `Core` (implémente les interfaces de repository).
- `Services` dépend de `Core` et `Data`.
- `ViewModels` dépend de `Core` et `Services` (jamais de `Data` directement).
- `Views` dépend de `ViewModels` (uniquement pour le typage du `DataContext`, pas de logique).
- `App` est le seul projet à référencer tout le monde : c'est le composition root (DI) et le point d'entrée.
- `Tests` référence les projets qu'il teste (`Core`, `Data`, `Services`, `ViewModels`).

## Conventions de code
- Langage du domaine : les entités et le vocabulaire métier restent en **français**
  (`Tache`, `Priorite`, `Statut`, `Categorie`, …) car c'est la langue du produit.
  Le vocabulaire technique/infrastructure reste en anglais standard .NET
  (`Repository`, `DbContext`, `Add`, `GetByIdAsync`, …).
- Nommage : PascalCase pour les membres publics et les types, `_camelCase` pour
  les champs privés, suffixe `Async` pour toute méthode asynchrone.
- ViewModels : `ObservableObject` + attributs `[ObservableProperty]` /
  `[RelayCommand]` de CommunityToolkit.Mvvm plutôt que du code manuel de
  `INotifyPropertyChanged`.
- Repository pattern : interfaces dans `Core` (`ITacheRepository`),
  implémentations dans `Data`. Les couches au-dessus de `Data` ne connaissent
  jamais EF Core directement.
- Un projet par responsabilité (voir structure ci-dessus) ; pas de logique
  métier dans les code-behind XAML.
- Tests xUnit : un fichier de test par classe testée, structure
  Arrange/Act/Assert, nommage `MethodName_Scenario_ExpectedResult`.
- Commits atomiques par étape logique (un projet ajouté, une entité, une vue,
  des tests, etc.), pas de gros commit final fourre-tout.

## Découpage en sprints
- **Sprint 1 — Fondations** : solution .NET, DI, CommunityToolkit.Mvvm, EF Core
  + SQLite, entité `Tache`, Repository pattern, fenêtre principale (formulaire
  de création + liste), tests unitaires de base, README.
- **Sprints suivants** (à détailler en temps voulu) : filtres/tri/recherche,
  vue planning (calendrier + drag & drop), rappels et notifications toast,
  thème clair/sombre.

Chaque sprint est traité comme un epic indépendant : ne pas anticiper le
code des sprints suivants tant qu'il n'a pas été explicitement démarré.
