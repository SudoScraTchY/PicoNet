# PicoNet — Documentation

This docs folder summarizes the project architecture, runtime components and developer setup for the PicoNet solution. Use the per-area pages for more details:

- architecture.md — high-level architecture and module responsibilities
- services.md — external services (Postgres, Redis, JWT, AppHost parameters)
- database.md — schema highlights, migrations and vector search setup
- caching.md — Redis cache design and TTL / hit counting
- events.md — event-driven design and Wolverine usage
- ui.md — Blazor Interactive Server notes and how the UI connects
- developer-setup.md — commands and environment variables to run and test locally
- user-and-role-management.md — comprehensive API documentation for user and role management endpoints

Notes in this documentation are generated from the workspace source code. Paths and symbols in this repo referenced below include: `src/PicoNet.Api`, `src/PicoNet.Application`, `src/PicoNet.Infrastructure`, `src/PicoNet.UI`, and `src/PicoNet.AppHost`.

