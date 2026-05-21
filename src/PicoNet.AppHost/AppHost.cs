// PicoNet.AppHost/Program.cs
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// == 1. Add PostgreSQL Database ==
// The password should ideally come from a secure parameter or user secrets.
var dbPassword = builder.AddParameter("postgres-password", secret: true);
var postgres = builder
    .AddPostgres("piconet-db", password: dbPassword, port: 5432)
    .WithDataVolume("piconet-postgres-data") // Persist data across container restarts
    .WithPgAdmin() // Adds a web-based admin tool
    .AddDatabase("piconet"); // The actual database to use in connection strings

// == 2. Add Garnet Cache ==
// Garnet is a high-performance, drop-in replacement for Redis.
// Learn more: https://microsoft.github.io/garnet/
var cache = builder
    .AddGarnet("piconet-cache", port: 6379)
    .WithDataVolume("piconet-garnet-data") // Persist cache data
    .WithPersistence(interval: TimeSpan.FromMinutes(5), keysChangedThreshold: 100); // Snapshot persistence

// == 3. Add Your Services ==

// The Blazor UI project
var ui = builder
    .AddProject<Projects.PicoNet_UI>("ui")
    .WithReference(postgres) // Injects the PostgreSQL connection string
    .WithReference(cache)    // Injects the Garnet connection string
    .WithExternalHttpEndpoints(); // Makes the UI accessible from outside

// The API project
var api = builder
    .AddProject<Projects.PicoNet_Api>("api")
    .WithReference(postgres)
    .WithReference(cache)
    .WithExternalHttpEndpoints();

// == 4. Build and Run ==
// When you run the AppHost project, all defined services will start automatically.
builder.Build().Run();