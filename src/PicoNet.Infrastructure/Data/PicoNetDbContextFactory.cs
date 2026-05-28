// PicoNet.Infrastructure/Data/PicoNetDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PicoNet.Infrastructure.Data;

// public class PicoNetDbContextFactory : IDesignTimeDbContextFactory<PicoNetDbContext>
// {
//     public PicoNetDbContext CreateDbContext(string[] args)
//     {
//         // Build configuration
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "PicoNet.Api"))
//             .AddJsonFile("appsettings.json", optional: false)
//             .AddJsonFile("appsettings.Development.json", optional: true)
//             .Build();
//         
//         var optionsBuilder = new DbContextOptionsBuilder<PicoNetDbContext>();
//         
//         optionsBuilder.UseNpgsql(
//             configuration.GetConnectionString("DefaultConnection"),
//             npgsqlOptions =>
//             {
//                 npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
//             });
//         
//         return new PicoNetDbContext(optionsBuilder.Options);
//     }
// }