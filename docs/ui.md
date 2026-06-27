# UI (Blazor Interactive Server)

The UI project (`src/PicoNet.UI`) is built with Blazor Interactive Server components.

Key notes
- The UI registers distributed caching via `builder.AddRedisDistributedCache` and expects the Redis connection string from configuration or environment variables (`ConnectionStrings__piconet-cache` or `REDIS_CONNECTION_STRING`).
- UI calls backend APIs via typed API clients (interfaces under `PicoNet.UI/ApiClients/Interfaces`, implementations under `ApiClients/Implementations`).
- Interactive server components are registered with:
  builder.Services.AddRazorComponents().AddInteractiveServerComponents();

Routing and pages
- The admin pages for creating and listing links live under `PicoNet.UI/Components/Admin` (e.g., `CreateUrl.razor`, `Dashboard.razor`).
- The UI uses `IUrlApiClient` and `IRedirectClient` to call the API endpoints.

Authentication
- The UI integrates authentication helpers (see `BitzArt.Blazor.Auth.Server` usage in `Program.cs`) and stores tokens in a scoped `ICircuitTokenStore`.

Developer notes
- In development the UI uses service discovery helpers to call the API by logical name. In production, typical configuration would point to known hostnames.
- Ensure Redis connection string is available to the UI during local runs to avoid startup errors (the code throws if Redis connection string is not found).

