using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TalaanNiAna.Infrastructure.Data;

/// <summary>Used by dotnet-ef at design time, where there is no request and no store.</summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TalaanNiAna.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new AppDbContext(optionsBuilder.Options, new NoStore());
    }

    /// <summary>Migrations only build the model, so no store context is needed.</summary>
    private sealed class NoStore : ICurrentStore
    {
        public int? StoreId => null;
        public int Required => throw new InvalidOperationException("No store at design time.");
    }
}
