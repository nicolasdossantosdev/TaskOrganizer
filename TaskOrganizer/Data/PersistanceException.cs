namespace TaskOrganizer.Data;

/// <summary>
/// Erreur d'accès aux données destinée à être affichée à l'utilisateur
/// (message déjà formulé en langage clair, sans détail technique EF Core/SQLite).
/// </summary>
public class PersistanceException : Exception
{
    public PersistanceException(string message)
        : base(message)
    {
    }

    public PersistanceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
