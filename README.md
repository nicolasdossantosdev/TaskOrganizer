# TaskOrganizer

Application desktop Windows (WPF / .NET) pour organiser des tâches : création,
planification, priorisation et suivi de statut. Voir [CLAUDE.md](CLAUDE.md)
pour le contexte produit, le périmètre et les conventions du projet.

## Statut

- **Sprint 1 (Fondations)** : CRUD de base des tâches, persistance SQLite via
  EF Core, fenêtre principale avec formulaire de création et liste des tâches.
- **Sprint 2** : édition/suppression de tâche, changement de statut inline,
  catégories multiples, recherche et tri dans la liste, rappels programmables
  par tâche avec notifications toast Windows natives et résumé des rappels
  manqués au démarrage.
- **Sprint 3** : onglet Planning avec bascule Jour/Semaine/Mois/Aujourd'hui/
  À venir, replanification d'une tâche par glisser-déposer directement sur
  le calendrier.

## Prérequis

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) ou supérieur
- Windows (l'application utilise WPF, spécifique à Windows)
- Le outil `dotnet-ef` pour gérer les migrations (installation ci-dessous)

## Structure du projet

```
TaskOrganizer.slnx
TaskOrganizer/            Application WPF (Models, Data, Services, ViewModels, Views)
TaskOrganizer.Tests/      Tests unitaires xUnit
```

## Builder le projet

```powershell
dotnet build TaskOrganizer.slnx
```

## Lancer l'application

```powershell
dotnet run --project TaskOrganizer
```

Au premier démarrage, l'application applique automatiquement les migrations
EF Core et crée la base SQLite dans
`%LOCALAPPDATA%\TaskOrganizer\taskorganizer.db`.

Les rappels sont scrutés toutes les 30 secondes en arrière-plan et déclenchent
une notification toast native (Centre de notifications Windows). Un résumé
des rappels manqués (dus pendant que l'application était fermée) s'affiche
au démarrage s'il y en a.

## Lancer les tests

```powershell
dotnet test TaskOrganizer.slnx
```

## Gérer les migrations EF Core

Installer l'outil `dotnet-ef` (une seule fois) :

```powershell
dotnet tool install --global dotnet-ef
```

Ajouter une nouvelle migration après une modification du modèle
(`TaskOrganizer/Models`) :

```powershell
dotnet ef migrations add NomDeLaMigration --project TaskOrganizer --startup-project TaskOrganizer -o Data/Migrations
```

Les migrations sont appliquées automatiquement au démarrage de
l'application ; il n'est pas nécessaire d'exécuter `dotnet ef database
update` manuellement en développement.
