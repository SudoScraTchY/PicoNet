# Developer Setup and Quick Start

This section collects the most useful commands and environment variables to run PicoNet locally and to perform common developer tasks.

Prerequisites
- .NET SDK (the project uses .NET 8+ artifacts in project files — verify with `dotnet --version`).
- Docker (if you want to run Postgres/Redis via AppHost or docker-compose).
- Optional: `dotnet-ef` tool for migrations (install with `dotnet tool install --global dotnet-ef`).

Run API locally
- From the solution root or `src/PicoNet.Api` folder:

  dotnet run --project src/PicoNet.Api

- The API expects configuration for Postgres and Redis. You can run the AppHost to provide these services (see below) or set environment variables.

Run UI locally
- The UI will fail startup if it cannot find a Redis connection string. Provide one of:
  - `ConnectionStrings__piconet-cache` environment variable
  - `REDIS_CONNECTION_STRING` environment variable

  Example (PowerShell):

  $env:ConnectionStrings__piconet-cache = "localhost:6379"
  dotnet run --project src/PicoNet.UI

Running AppHost (recommended for local end-to-end)
- AppHost composes Postgres and Redis for local development using pinned container images. To run the AppHost:

  dotnet run --project src/PicoNet.AppHost

- AppHost expects two secret parameters that it declares: `postgres-password` and `jwt-key`. Depending on the AppHost CLI or framework, set these when prompted or via environment variables used by the AppHost runner.

Useful EF Core commands
- Add migration (run from solution root):

  dotnet ef migrations add <Name> -p src/PicoNet.Infrastructure -s src/PicoNet.Api

- Apply migrations:

  dotnet ef database update -p src/PicoNet.Infrastructure -s src/PicoNet.Api

Environment variables reference
- Database:
  - Connection string key: `ConnectionStrings:piconet` (used in `InfrastructureExtensions` and `PicoNetDbContextFactory`)
- Redis:
  - `ConnectionStrings__piconet-cache` or `REDIS_CONNECTION_STRING` for UI fallback
- JWT:
  - `Jwt__Key` — secret signing key
  - `Jwt__Issuer` — e.g. `PicoNet`
  - `Jwt__Audience` — e.g. `PicoNetClients`

Notes on AppHost parameters
- `postgres-password` (secret): used by the Postgres container
- `jwt-key` (secret): mapped into `Jwt__Key` for the API when AppHost runs

Testing quick redirect flow
1. Start AppHost or ensure Postgres and Redis are running and reachable by API/UI.
2. Create an account (use API or UI) and create a short link via the UI or direct API call to `POST /api/shortener`.
3. Resolve a short URL via `GET /api/redirect/{shortCode}` — on first hit DB path will populate cache, subsequent hits should hit Redis.

Where to look for code of interest
- Fast-path cache: `src/PicoNet.Infrastructure/Cache/RedirectCacheService.cs` and `src/PicoNet.Application/Features/Redirect/Handler/RedirectHandler.cs`.
- Short code generation: `src/PicoNet.Infrastructure/Services/ShortCodeGenerator.cs`.
- Events & handlers: `src/PicoNet.Domain/Events`, `src/PicoNet.Application/Events/Handler/UrlVisitedEventHandler.cs`.
- Wolverine setup: `src/PicoNet.Api/Program.cs`.

If you want, I can also:
- Generate a small sequence diagram showing request flow for redirects and create commands.
- Add runnable Docker Compose overrides to help with local development.

