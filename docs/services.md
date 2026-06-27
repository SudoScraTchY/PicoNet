# External Services and Runtime Wiring

This file documents how Postgres, Redis and JWT are wired in the repository and how the AppHost composes them for local runs.

AppHost
- `src/PicoNet.AppHost/AppHost.cs` composes services for local development using a `DistributedApplication` builder.
- Parameters are declared in AppHost and can be supplied at runtime: `postgres-password` (secret) and `jwt-key` (secret).
- The AppHost creates pinned images and data volumes for Postgres and Redis (see `.WithImage(...)` and `.WithDataVolume(...)`).

Postgres
- A Postgres instance is added as `piconet-db` and a database named `piconet` is created by the AppHost.
- EF Core DbContext: `PicoNet.Infrastructure.Data.PicoNetDbContext` (registered in `InfrastructureExtensions.AddInfrastructure`).
- Connection strings used by the API are named `piconet` or `DefaultConnection` in configuration (see `PicoNetDbContextFactory` used for design-time tools).

Redis
- A Redis service is added as `piconet-cache` in AppHost.
- Redis connection string is expected by the UI in several places:
  - `appsettings.json` connection string `piconet-cache`
  - or environment variables `ConnectionStrings__piconet-cache` or `REDIS_CONNECTION_STRING` (see `PicoNet.UI/Program.cs`).
- The API registers a distributed Redis cache via `builder.AddRedisDistributedCache(connectionName: "piconet-cache")` in `PicoNet.Api/Program.cs` and registers `IRedirectCacheService`.

JWT and Secrets
- AppHost exposes env var mapping for JWT:
  - `Jwt__Key` (secret parameter `jwt-key`),
  - `Jwt__Issuer` = "PicoNet",
  - `Jwt__Audience` = "PicoNetClients".
- The API config expects JWT configuration in configuration under `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` — AppHost maps these as environment variables using double-underscore notation (`Jwt__Key`).

Wiring and Service Discovery
- In development the UI registers service discovery helpers (see `PicoNet.UI/Program.cs` where `AddServiceDiscovery` is used in development). The AppHost environment provides service endpoints when run locally through its builder.

Important environment variables and configuration keys
- Connection string keys: `ConnectionStrings:piconet` (Postgres), `ConnectionStrings:piconet-cache` (Redis)
- Redis fallback env vars: `ConnectionStrings__piconet-cache`, `REDIS_CONNECTION_STRING`
- Secrets: `postgres-password`, `jwt-key` (declared in `AppHost.cs`)
- JWT env names: `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`

If you run the AppHost (distributed application), it will create Postgres and Redis containers and map environment variables into the API and UI projects.

