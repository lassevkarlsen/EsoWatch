using EsoWatch.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Data;

public class EsoDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public EsoDbContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = _configuration["ConnectionString"];
        if (connectionString is null)
        {
            throw new InvalidOperationException();
        }

        optionsBuilder.UseNpgsql(connectionString);
    }

    public DbSet<GenericTimer> Timers { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
}