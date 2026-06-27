on# Architecture Overview

PicoNet is structured as a classic layered .NET application, separated into responsibilities:

- PicoNet.Api — HTTP API surface and controllers (controllers use Wolverine to invoke application commands/queries)
- PicoNet.Application — Application layer containing commands, queries and handlers (business flows)
- PicoNet.Contracts — DTOs used between API and UI
- PicoNet.Domain — Entities, value objects and domain events
- PicoNet.Infrastructure — EF Core, caching, data access, short-code generation and service implementations
- PicoNet.UI — Blazor Interactive Server frontend
- PicoNet.AppHost — Local composition for dependencies and containers (Postgres, Redis) used for development with pinned images

Key points:
- Event-driven flows use the Wolverine library (see `PicoNet.Api` Program.cs where `AddWolverine` is called).
- Controllers create and dispatch application commands through an `IMessageBus` (Wolverine) — e.g. `ShortenerController` and `RedirectController`.
- Domain events are raised from domain entities (e.g. `UrlCreatedDomainEvent`) and handled in the application event handlers (wired via Wolverine).
- Persistence uses EF Core with PostgreSQL — the `PicoNetDbContext` lives in `PicoNet.Infrastructure.Data`.
- Caching / redirect fast-path uses Redis via `IDistributedCache` and `RedirectCacheService`.
- Short codes are value objects (`PicoNet.Domain.ValueObjects.ShortCode`), and a `ShortCodeGenerator` exists in `PicoNet.Infrastructure.Services` (uses Hashids).

This layered design keeps controllers thin (they map HTTP to commands), handlers implement business logic and infrastructure provides concrete implementations.

