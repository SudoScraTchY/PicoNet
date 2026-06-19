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
    .WithDataVolume("piconet-redis-data")
    .WithRedisCommander();

// The API project
// AppHost/Program.cs
var jwtKey = builder.AddParameter("jwt-key", secret: true);

var api = builder
    .AddProject<Projects.PicoNet_Api>("api")
    .WithReference(postgres)
    .WithReference(cache)
    .WithEnvironment("Jwt__Key", jwtKey)        // double underscore — see below
    .WithEnvironment("Jwt__Issuer", "PicoNet")
    .WithEnvironment("Jwt__Audience", "PicoNetClients")
    .WaitFor(postgres)
    .WaitFor(cache)
    .WithExternalHttpEndpoints();

// The Blazor UI project
var ui = builder
    .AddProject<Projects.PicoNet_UI>("ui")
    .WithReference(api)
    .WithReference(postgres) 
    .WithReference(cache)    // Automatically maps to the new Redis connection string
    .WaitFor(api)
    .WithExternalHttpEndpoints(); 

// == 4. Build and Run ==
builder.Build().Run();