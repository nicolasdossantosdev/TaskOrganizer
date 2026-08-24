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

Solution à 2 projets : une application WPF unique organisée par dossiers, et
un projet de tests séparé. Pas de multi-projet par couche (Core/Data/...) :
la séparation des responsabilités se fait par dossier et par convention de
dépendance, pas par assembly.

```
TaskOrganizer.slnx
CLAUDE.md
README.md
TaskOrganizer/              WPF, exécutable, composition root (DI + App.xaml)
  App.xaml / App.xaml.cs    Point d'entrée, construction du ServiceProvider (DI).
  Models/                   Entités et enums du domaine (Tache, PrioriteTache, StatutTache).
  Data/                     EF Core : AppDbContext, Migrations, ITacheRepository + implémentation.
  Services/                 Logique applicative (orchestration, futurs rappels/notifications).
  ViewModels/               ViewModels MVVM (CommunityToolkit.Mvvm).
  Views/                    Fenêtres/UserControls XAML. Pas de logique métier dans le code-behind.
TaskOrganizer.Tests/        Tests xUnit (Data, Services, ViewModels).
```

Sens des dépendances (par convention, au sein du même projet) :

```
Views ──> ViewModels ──> Services ──> Data ──> Models
                                        ^
                                        └── Models (entités utilisées partout)
```

- `Models` ne dépend de rien d'autre (entités POCO + enums).
- `Data` dépend de `Models` (EF Core, `ITacheRepository`/`TacheRepository`).
- `Services` dépend de `Models` et `Data`.
- `ViewModels` dépend de `Models` et `Services` (jamais de `Data` directement).
- `Views` dépend de `ViewModels` (uniquement pour le typage du `DataContext`, pas de logique).
- `App.xaml.cs` est le seul point qui connaît tout : c'est le composition root (DI),
  et le seul endroit qui gère le cycle de vie du `RappelBackgroundService`.
- `TaskOrganizer.Tests` référence le projet `TaskOrganizer` dans son ensemble.

**Exception assumée au sens des dépendances** : `IDialogService` (fenêtre
d'édition, confirmations, message d'erreur) est *défini* dans `ViewModels`
mais *implémenté* dans `Views` (`DialogService`, qui ouvre de vraies
`Window`/`MessageBox`) et injecté via DI. Ça permet à `MainViewModel` de
rester testable (voir `FakeDialogService` dans les tests) sans connaître
WPF, tout en gardant l'ouverture de fenêtres du côté Views.

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
- Repository pattern : interface et implémentation vivent toutes les deux
  dans `Data` (`ITacheRepository`/`TacheRepository`, `ICategorieRepository`/
  `CategorieRepository`, `IRappelRepository`/`RappelRepository`). Les couches
  au-dessus (`Services`, `ViewModels`) dépendent de l'interface, jamais
  d'EF Core directement. Tous les repositories héritent de `RepositoryBase`
  (retry sur erreurs SQLite transitoires + `PersistanceException` pour les
  erreurs définitives, voir Sprint 2).
- Formulaires de tâche : `TacheFormViewModelBase` centralise validation,
  parsing des catégories et logique de rappels ; `CreateTacheViewModel` et
  `EditTacheViewModel` n'implémentent que `PersisterAsync` (Create vs Update).
- Un dossier par responsabilité (voir structure ci-dessus) ; pas de logique
  métier dans les code-behind XAML.
- Tests xUnit : un fichier de test par classe testée, structure
  Arrange/Act/Assert, nommage `MethodName_Scenario_ExpectedResult`.
- Commits atomiques par étape logique (un projet ajouté, une entité, une vue,
  des tests, etc.), pas de gros commit final fourre-tout.

## Découpage en sprints
- **Sprint 1 — Fondations** : solution .NET, DI, CommunityToolkit.Mvvm, EF Core
  + SQLite, entité `Tache`, Repository pattern, fenêtre principale (formulaire
  de création + liste), tests unitaires de base, README.
- **Sprint 2 — fin de l'epic Gestion des tâches + epic Reminders & Notifications** :
  édition et suppression (avec confirmation) d'une tâche, changement de statut
  inline depuis la liste, catégories multiples (`Categorie`, relation
  many-to-many), recherche texte + tri dans la liste (`ICollectionView`),
  rappels programmables par tâche (`Rappel`, offsets 1h avant / la veille / 1
  semaine avant), `RappelBackgroundService` (scrutation périodique) +
  notifications toast Windows natives (`Microsoft.Toolkit.Uwp.Notifications`,
  TFM `net10.0-windows10.0.19041.0`), résumé des rappels manqués au démarrage,
  fiabilisation de la persistance (`RepositoryBase` : retry + `PersistanceException`).
- **Sprint 3 — epic Planning** : vue Planning (`PlanningView`, `PlanningViewModel`)
  avec bascule Jour/Semaine/Mois/Aujourd'hui/À venir (`ModePlanning`),
  positionnement des tâches sur le calendrier via `IPlanningService`
  (calcul pur, sans persistance, réutilise `ITacheService.ObtenirToutesAsync`
  déjà existant), replanification par glisser-déposer (met à jour
  `DateEcheance` et appelle `ITacheService.ModifierAsync`, avec revert en
  mémoire sur `PersistanceException`). "Aujourd'hui" et "À venir" sont des
  modes de la même vue plutôt que des écrans séparés, pour ne pas dupliquer
  le rendu ni la logique de regroupement par jour.
- **Sprints suivants** (à détailler en temps voulu) : thème clair/sombre.

Chaque sprint est traité comme un epic indépendant : ne pas anticiper le
code des sprints suivants tant qu'il n'a pas été explicitement démarré.
