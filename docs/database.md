# Database

This section highlights important database design and search optimizations.

DbContext and Migrations
- DbContext: `PicoNet.Infrastructure.Data.PicoNetDbContext` (registered in `InfrastructureExtensions.AddInfrastructure`).
- There is a design-time factory `PicoNetDbContextFactory` which points at the `PicoNet.Api` configuration files for migrations/tools.
- Migrations are present in `PicoNet.Infrastructure/Migrations` (including `Initial` and `PicoNetDbContextModelSnapshot`).

ShortenedUrl entity
- The main table `ShortenedUrls` stores the short link. Key columns:
  - `NanoId` (value object `ShortCode` mapped to a string, max length 20) — this is the short-code used for redirects and is indexed unique.
  - `OriginalUrl` (up to 2048 chars)
  - `CustomAlias` (optional user-provided alias, unique when present)
  - `Tags` (comma-separated), `Campaign`, `Password`, `MaxClicks`, `ClickCount`, `ExpiryTime`, `IsPermanent`, etc.

Search vector (full-text) and performance
- A computed tsvector column `SearchVector` is defined (type `tsvector`) and computed using:
  to_tsvector('english', coalesce("OriginalUrl", '') || ' ' || coalesce("CustomAlias", '') || ' ' || coalesce("Tags", ''))
- This enables faster full-text search queries across OriginalUrl, CustomAlias and Tags. The migration and model snapshot show `NpgsqlTsVector` mapping.

ShortCode as value object
- `PicoNet.Domain.ValueObjects.ShortCode` wraps string values and validates allowed characters and length.
- `ShortenedUrl.NanoId` is mapped via a conversion in `ShortenedUrlConfiguration`:
  - code => code.Value
  - value => new ShortCode(value)

Indexes
- `NanoId` has a unique index (`IX_shortened_urls_nano_id`).
- `CustomAlias` has a unique filtered index when not null.
- Other helpful indexes exist for `CreatedAt`, `ExpiryTime`, and UrlVisits.

Design notes
- Using the ShortCode as a value object mapped to a string gives type-safety in the domain and allows EF Core to persist the underlying string.
- The `SearchVector` computed column provides efficient searching for UI listing and admin search features. If you extend search, consider adding GIN indexes over `SearchVector` in migrations for large datasets.

Running Migrations (example commands)
- From a developer machine, using the design-time factory, you can run EF Core commands (from the `src` folder or root):

  dotnet ef migrations add <Name> -p src/PicoNet.Infrastructure -s src/PicoNet.Api
  dotnet ef database update -p src/PicoNet.Infrastructure -s src/PicoNet.Api

The `-s` (startup project) ensures the appsettings used by the factory are found.

