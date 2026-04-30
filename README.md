# rent_for_students

A web service for students to search and rent housing during their studies. Landlords can post listings; students can browse, apply, and manage rental applications.

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC |
| Language | C# |
| ORM | EF Core |
| Database | Microsoft SQL Server (Windows Authentication) |
| Testing | xUnit |

## Architecture

Layered MVC with explicit separation of concerns:

```
Controllers  →  Commands  →  CommandDispatcher  →  UseCaseMediator  →  DomainService / Repository  →  Result<T>
```

| Layer | Responsibility |
|---|---|
| `Controllers/` | HTTP routing, ViewModel binding, delegating to mediator via commands |
| `Application/Commands/` | One command per operation — explicit intent objects |
| `Application/UseCases/` | Use case orchestration: validate → execute → notify → return result |
| `Application/Common/` | `Result<T>`, `ErrorCodes` |
| `Domain/Entities/` | Business entities: `HousingListing`, `RentalApplication`, `RentalApplicationProfile` |
| `Domain/Services/` | Domain logic: `HousingService` |
| `Domain/Contracts/` | Repository interfaces, prototype interface, report repository |
| `Infrastructure/` | EF Core repositories, SP-backed repositories, `AppDbContext`, logging |

## Design Patterns

| Pattern | Version | Location |
|---|---|---|
| **Command** | active | `Application/Commands/` |
| **Mediator** | active | `Application/UseCases/`, `Controllers/` |
| **Template Method** | v1.2 | `Application/UseCases/BaseUseCaseMediator.cs` |
| **Prototype** | v1.4 | `Domain/Contracts/IRentalApplicationPrototype.cs`, `Domain/Entities/RentalApplicationProfile.cs` |
| **Flyweight** | v1.5 | `Domain/Flyweight/` — `RoomType` metadata (DisplayName, Description, TypicalCapacity, CssClass) |

## Database

SQL Server with EF Core migrations and stored procedures.

**Stored procedures** (`SQL/StoredProcedures/`):
- `01_HousingListings_CRUD.sql` — 5 SPs for listings
- `02_RentalApplications_CRUD.sql` — 6 SPs for applications
- `03_RentalApplicationProfiles_CRUD.sql` — 5 SPs for student profiles
- `04_View_And_CursorReport.sql` — SQL View + cursor-based demand report SP

**Connection string** (`appsettings.json`):
```json
"DefaultConnection": "Server=localhost;Database=RentForStudents;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Getting Started

**Prerequisites:** .NET 8 SDK, SQL Server (local, Windows Authentication)

```bash
# Restore and build
dotnet build

# Apply EF Core migrations
dotnet ef database update

# Run stored procedures manually from SQL/StoredProcedures/ in order:
# 01 → 02 → 03 → 04

# Run the app
dotnet run
```

## Running Tests

```bash
dotnet test
```

Tests use in-memory fakes (`InMemoryHousingRepository`, `StubListingReportRepository`) — no real database required.

## Project Structure

```
/
├── Application/
│   ├── Commands/          # Command objects (one per operation)
│   ├── Common/            # Result<T>, ErrorCodes
│   ├── Notifications/     # INotificationService
│   └── UseCases/          # Mediators (BaseUseCaseMediator, ListingUseCaseMediator, ...)
├── Controllers/           # MVC controllers
├── Domain/
│   ├── Contracts/         # Repository and prototype interfaces
│   ├── Entities/          # HousingListing, RentalApplication, RentalApplicationProfile
│   ├── Flyweight/         # RoomType flyweight + factory
│   ├── Reports/           # Report row models
│   ├── Requests/          # Search criteria / input models
│   └── Services/          # HousingService
├── Infrastructure/
│   ├── Repositories/      # EF Core + SP-backed repository implementations
│   └── Migrations/        # EF Core migrations
├── SQL/StoredProcedures/  # SQL scripts (SPs, Views)
├── UML/                   # PlantUML diagrams by pattern
├── ViewModels/            # MVC view models
├── Views/                 # Razor views
├── tests/                 # xUnit test project
└── docs/                  # AI context, ADRs, session logs
```

## Version History

| Version | Description |
|---|---|
| v1.7 | Demand report: SQL View + cursor SP, `ReportsController`, `IListingReportRepository` |
| v1.6 | Database migration SQLite → SQL Server, 16 stored procedures |
| v1.5 | Flyweight pattern: `RoomType` as rich Flyweight with factory (Variant 1A) |
| v1.4 | Prototype pattern: baseline interface-based prototype for rental application profiles |
| v1.2 | Template Method: `BaseUseCaseMediator` with validate/execute/notify skeleton |
| v1.0 | Command + Mediator baseline |
