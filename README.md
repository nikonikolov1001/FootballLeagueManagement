# Football League Management

ASP.NET Core 8 project for managing an English Premier League API.

## Open in Visual Studio

1. Open Visual Studio 2022 or newer.
2. Choose **Open a project or solution**.
3. Select `FootballLeagueManagement.sln`.
4. Press **F5**.

The app uses SQL Server LocalDB:

```text
Server=(localdb)\mssqllocaldb;Database=FootballLeagueManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

In Development mode, the app applies EF Core migrations automatically on startup.

## Architecture

- `FootballLeagueManagement` - ASP.NET Core 8 Web/API project.
- `FootballLeagueManagement.Core` - entity models, DTOs, and service interfaces.
- `FootballLeagueManagement.Infrastructure` - EF Core, SQL Server, Identity, migrations, seeded data, services.
- `FootballLeagueManagement.Tests` - NUnit test project prepared separately. Add it back to the solution after NuGet restore is available.

## Covered Requirements

- ASP.NET Core 8.
- Web API with more than 10 endpoints.
- 4+ entity models: `Club`, `Player`, `Stadium`, `Match`, plus `ApplicationUser`.
- 4+ controllers: `Clubs`, `Players`, `Stadiums`, `Matches`, `League`, `Account`.
- SQL Server connection string.
- EF Core DbContext, migration files, and seeded data.
- CRUD endpoints for clubs, players, stadiums, and matches.
- Input validation with data annotations and API model validation.
- Business logic in services: standings and result recording.
- Dependency injection.
- ASP.NET Identity with custom user properties: `FullName`, `FavoriteClub`.
- Seeded Administrator role and admin user.
- Admin-only create/update/delete endpoints.
- AJAX/fetch dashboard through `wwwroot/js/site.js`.

## Seeded Admin

```text
Email: admin@premierleague.local
Password: Admin123!
Role: Administrator
```

## Main API Endpoints

- `GET /api/league/summary`
- `GET /api/league/standings`
- `GET /api/clubs`
- `GET /api/clubs/{id}`
- `POST /api/clubs`
- `PUT /api/clubs/{id}`
- `DELETE /api/clubs/{id}`
- `GET /api/players`
- `GET /api/stadiums`
- `GET /api/matches`
- `PATCH /api/matches/{id}/result`
- `POST /api/account/register`
- `POST /api/account/login`
- `POST /api/account/logout`
- `GET /api/account/profile`

## Tests

`FootballLeagueManagement.Tests` contains NUnit tests for `MatchService` and `LeagueQueryService`.

The current restricted environment could not download NUnit/Moq/EF InMemory packages from NuGet, so the test project is present on disk but not included in the main `.sln` build. After NuGet works in Visual Studio, add it back:

```powershell
dotnet sln FootballLeagueManagement.sln add FootballLeagueManagement.Tests\FootballLeagueManagement.Tests.csproj
dotnet test
```

## Source Control Note

The exam document requires a public GitHub repository with 25 commits across 7 different days. That history cannot be generated honestly in one session; create the GitHub repository early and commit your real progress over time.
