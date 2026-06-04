using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrbitalAcademy.Infrastructure.Persistence;

public sealed class DesignTimeOrbitalAcademyDbContextFactory
    : IDesignTimeDbContextFactory<OrbitalAcademyDbContext>
{
    public OrbitalAcademyDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__OrbitalAcademy")
            ?? "Host=localhost;Port=5432;Database=orbital_academy;Username=orbital;Password=orbital";

        DbContextOptions<OrbitalAcademyDbContext> options = new DbContextOptionsBuilder<OrbitalAcademyDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new OrbitalAcademyDbContext(options);
    }
}
