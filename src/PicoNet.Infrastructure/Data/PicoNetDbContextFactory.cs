using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PicoNet.Infrastructure.Data;

public sealed class PicoNetDbContextFactory
    : IDesignTimeDbContextFactory<PicoNetDbContext>
{
    public PicoNetDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine( Directory.GetCurrentDirectory(), "../PicoNet.Api"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .Build();

        var dbConnectionString = 
            configuration.GetConnectionString("piconet") ??
            configuration.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<PicoNetDbContext>();

        options.UseNpgsql(dbConnectionString);

        return new PicoNetDbContext(options.Options);
    }
}