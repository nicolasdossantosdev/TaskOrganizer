using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskOrganizer.Data;

/// <summary>
/// Used by the `dotnet ef` tooling to create migrations without needing the
/// full DI-configured application to run.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=taskorganizer.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}
