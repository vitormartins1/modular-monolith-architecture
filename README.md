# Evently — Modular Monolith Architecture

Overview
--------
Evently is an example modular-monolith application implemented with .NET 8. The solution keeps a single deployable application while organizing domain functionality into well-defined modules (Events, Users, etc.) using clean architecture principles (Presentation, Application, Domain, Infrastructure).

Key features (from commit history)
----------------------------------
- Modular structure with per-module projects and clear boundaries.
- CQRS-style application layer with commands, queries and MediatR handlers.
- Domain events infrastructure: domain events implement MediatR notifications and are published automatically via a DbContext interceptor.
- Pipeline behaviors for MediatR: validation, request logging and centralized exception handling.
- Redis caching with fallback to in-memory cache when Redis is unavailable.
- Health checks for PostgreSQL and Redis, exposed on a health endpoint compatible with HealthChecks.UI.
- Serilog structured logging with optional Seq sink configuration.
- Centralized endpoint registration using a shared `IEndpoint` abstraction for discoverability across modules.
- EF Core migrations maintained per module (Events, Users), with example migration files included.

Repository layout (high level)
------------------------------
- `Evently.Api` — API host and composition root (Program.cs). Registers modules, health checks, infrastructure, logging and endpoints.
- `Evently.Modules.Events.*` — Events module split into:
  - `Evently.Modules.Events.Domain`
  - `Evently.Modules.Events.Application`
  - `Evently.Modules.Events.Infrastructure`
  - `Evently.Modules.Events.Presentation`
- `Evently.Modules.Users.*` — Users module (Domain, Application, Infrastructure, Presentation).
- `Evently.Common.*` — Shared libraries for Domain, Application, Infrastructure and Presentation concerns.

Requirements
------------
- .NET 8 SDK
- `dotnet` CLI
- PostgreSQL (for production / development DB) — migrations target PostgreSQL
- Optional: Redis for distributed caching and caching examples
- Docker (if using provided Dockerfile / docker-compose)

Run locally
-----------
From the repository root:

1. Restore and build
   - `dotnet restore`
   - `dotnet build -c Release`

2. Run API host
   - `dotnet run --project Evently/Evently.Api`

Behavioral notes
----------------
- Program.cs loads module-specific configuration (e.g. `events`, `users`) and registers application assemblies with MediatR and FluentValidation.
- In Development, the host applies EF Core migrations automatically (`ApplyMigrations()` call in `Program.cs`).
- Health endpoint is mapped at `/health` and returns a UI-compatible JSON payload.
- Endpoints are discovered via implementations of the `IEndpoint` interface in each module.

Docker
------
- `Evently.Api/Dockerfile` builds the host image and exposes ports `8080` and `8081`.
- Example build command:
  - `docker build -f Evently/Evently.Api/Dockerfile -t evently:latest .`

EF Core migrations
------------------
- Migrations are located in module infrastructure projects (for example `Evently.Modules.Events.Infrastructure`).
- Add a migration example:
  - `dotnet ef migrations add AddSomething --project Evently/Evently.Modules.Events.Infrastructure --startup-project Evently/Evently.Api`
- Ensure `dotnet-ef` tool is installed if you run migrations locally:
  - `dotnet tool install --global dotnet-ef`

Configuration
-------------
- `Program.cs` reads connection strings from configuration keys `ConnectionStrings:Database` and `ConnectionStrings:Cache`.
- Module-specific JSON configuration files are supported and registered by the configuration extensions (e.g. `modules.events.json`).

Development notes
-----------------
- Logging: Serilog is configured in `Program.cs`. Optionally configure Seq by updating `appsettings.json`.
- Domain events: the `PublishDomainEventsInterceptor` is registered to publish domain events raised by entities after a successful save.
- Caching: an `ICacheService` uses Redis when available and falls back to in-memory cache on connection failure.
- Validation: FluentValidation validators are discovered and registered automatically for requests handled via MediatR.

Testing
-------
- Run tests (if test projects are present):
  - `dotnet test`
- When adding tests, follow the per-module convention (e.g. `tests/Evently.Modules.Events.Tests`).
