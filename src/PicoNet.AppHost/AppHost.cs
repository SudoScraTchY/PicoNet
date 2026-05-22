// PicoNet.AppHost/Program.cs
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// == 1. Add PostgreSQL Database (Pinned to 17.6) ==
var dbPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder
    .AddPostgres("piconet-db", password: dbPassword, port: 5432)
    // Pin exactly to postgres:17.6
    .WithImage("library/postgres", "17.6") 
    .WithDataVolume("piconet-postgres-data") 
    // Pin pgAdmin to your target version 9.12.0
    .WithPgAdmin(c => c.WithImage("dpage/pgadmin4", "9.12.0"))
    .AddDatabase("piconet"); 

// == 2. Add Redis Cache (Replacing Garnet, Pinned to stable v7 / v8) ==
var cache = builder
    .AddRedis("piconet-cache", port: 6379)
    .WithImage("library/redis", "8.6.3") 
    .WithDataVolume("piconet-redis-data");

// == 3. Add Redis Commander UI ==
// Aspire doesn't have a native extension for Redis Commander yet,
// so we hook it up manually via an isolated utility container container wrapper.
builder.AddContainer("piconet-redis-commander", "rediscommander/redis-commander", "latest")
    .WaitFor(cache)
    // Map Redis Commander to run on host port 8081
    .WithHttpEndpoint(targetPort: 8081, port: 8081, name: "commander-ui")
    // Pass the connection string metadata directly from the Aspire Redis resource node
    .WithEnvironment("REDIS_HOSTS", () => $"piconet-cache:{cache.Resource.Name}:6379");

// The API project
var api = builder
    .AddProject<Projects.PicoNet_Api>("api")
    .WithReference(postgres)
    .WithReference(cache)
    .WaitFor(postgres)
    .WaitFor(cache)
    .WithExternalHttpEndpoints();
// The Blazor UI project
var ui = builder
    .AddProject<Projects.PicoNet_UI>("ui")
    .WithReference(postgres) 
    .WithReference(cache)    // Automatically maps to the new Redis connection string
    .WaitForCompletion(api)
    .WithExternalHttpEndpoints(); 

// == 4. Build and Run ==
builder.Build().Run();