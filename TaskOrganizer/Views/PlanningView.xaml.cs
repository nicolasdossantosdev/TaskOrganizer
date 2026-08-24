using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskOrganizer.Models;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer.Views;

/// <summary>
/// Code-behind limité au câblage du drag & drop (extraction de la tâche
/// déplacée et du jour cible), la replanification elle-même est déléguée à
/// <see cref="PlanningViewModel.ReplanifierCommand"/>.
/// </summary>
public partial class PlanningView : UserControl
{
    private Point _pointDeDepartDrag;

    public PlanningView()
    {
        InitializeComponent();
    }

    private void TacheChip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _pointDeDepartDrag = e.GetPosition(null);
    }

    private void TacheChip_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var positionActuelle = e.GetPosition(null);
        var deplacement = positionActuelle - _pointDeDepartDrag;

        if (Math.Abs(deplacement.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(deplacement.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        if (sender is FrameworkElement { DataContext: Tache tache } element)
        {
            DragDrop.DoDragDrop(element, tache, DragDropEffects.Move);
        }
    }

    private void JourCard_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(typeof(Tache)))
        {
            return;
        }

        if (sender is not FrameworkElement { DataContext: JourPlanning jour }
            || e.Data.GetData(typeof(Tache)) is not Tache tache
            || DataContext is not PlanningViewModel viewModel)
        {
            return;
        }

        e.Handled = true;
        viewModel.ReplanifierCommand.Execute((tache, jour.Date));
    }
}
