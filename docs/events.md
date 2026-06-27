# Event-driven Design and Wolverine

Overview
- PicoNet uses an event-driven approach. The project integrates the Wolverine messaging library (see `PicoNet.Api/Program.cs` where `AddWolverine` is called and `AddWolverineHttp()` is chained).
- Controllers dispatch commands/queries through an `IMessageBus` (Wolverine). Handlers and event handlers respond to messages.

Where messages are dispatched
- Examples in controllers:
  - `ShortenerController` uses `_bus.InvokeAsync<ErrorOr<ShortUrlResponse>>(command)` to create or update short URL entries.
  - `RedirectController` uses `_bus.InvokeAsync<ErrorOr<RedirectUrlResult>>(command)` to resolve redirects.

Domain events -> application events
- Domain events (e.g. `UrlCreatedDomainEvent`) are raised by domain entities (see `ShortenedUrl.Create` which adds `UrlCreatedDomainEvent`).
- The application publishes or maps domain events to application events/commands that Wolverine will route to handlers.

Examples of application events and handlers
- `PicoNet.Application.Events.Commands.UrlVisitedEvent` is published when a URL is visited. The handler `UrlVisitedEventHandler`:
  - increments the global click counters in the database,
  - adds a `UrlVisit` record to the DB,
  - uses `IRedirectCacheService` when needed.
- `UrlCreatedDomainEvent` record exists in `PicoNet.Domain.Events` and contains the UrlId, ShortCode and original URL. These domain events are captured by the domain entity and passed into the pipeline for processing.

Wolverine configuration
- Wolverine is configured in `PicoNet.Api/Program.cs`. The code also enables FluentValidation for Command/Message validation:
  opts.Discovery.IncludeAssembly(typeof(ApplicationExtension).Assembly);
  opts.UseFluentValidation(cfg => cfg.RegistrationBehavior = RegistrationBehavior.ExplicitRegistration);

Message bus usage patterns
- Synchronous request/response: controllers call `_bus.InvokeAsync<TResponse>(command)` when they expect a result.
- Fire-and-forget / events: raise domain events or publish messages to decouple side-effects (persistence of visits, analytics tasks, etc.).

Extending events
- To add a new event handler:
  1. Create an event record in `PicoNet.Application.Events.Commands` (or domain events in `PicoNet.Domain.Events`).
  2. Implement a handler class under `PicoNet.Application.Events.Handler` with a `Handle(EventType evt, CancellationToken ct)` method.
  3. Wolverine will discover handlers based on assembly scanning (ensure the assembly is included in AddWolverine discovery).

Best practices
- Keep command handlers focused on state changes and raise domain events for side-effects.
- Use the cache fast-path for reads and use events / handlers to persist visit analytics asynchronously.
- Use explicit validation with FluentValidation for messages and register validators with Wolverine.

