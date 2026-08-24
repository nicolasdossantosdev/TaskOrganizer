namespace TaskOrganizer.Models;

public class Parametres
{
    public ThemeApplication Theme { get; set; } = ThemeApplication.Systeme;

    public bool SonNotificationsActif { get; set; } = true;
}
