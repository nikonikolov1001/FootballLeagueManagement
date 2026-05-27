# Football League Management System

ASP.NET Core 8 Web API project for managing a football league. The project uses SQL Server, Entity Framework Core, ASP.NET Identity, services, dependency injection, seed data, migrations, and NUnit tests.

## Main Features

- Manage teams, players, stadiums, and matches
- CRUD endpoints for the main entities
- Search teams and players
- Filter matches by finished/upcoming status
- Set match results
- ASP.NET Identity users with custom properties
- Administrator role for protected create, update, and delete actions
- SQL Server database with seeded football data
- NUnit tests with mocked dependencies

## Solution Structure

- `FootballLeagueManagement.Web` - Web API controllers and app startup
- `FootballLeagueManagement.Core` - entities, DTOs, contracts, and business services
- `FootballLeagueManagement.Infrastructure` - SQL Server DbContext, Identity user, repository, migrations, and dependency injection
- `FootballLeagueManagement.Tests` - NUnit tests

## Database

Default connection string:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=FootballLeagueManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Apply migrations:

```bash
dotnet ef database update --project FootballLeagueManagement.Infrastructure --startup-project FootballLeagueManagement.Web
```

## Admin User

The app creates this admin user when it starts:

- Email: `admin@football.local`
- Password: `Admin123`
- Role: `Administrator`

## Useful Endpoints

- `GET /api/teams`
- `GET /api/teams?search=Sofia`
- `POST /api/teams`
- `GET /api/players`
- `GET /api/players?teamId=1`
- `GET /api/stadiums`
- `GET /api/matches`
- `GET /api/matches?finished=true`
- `PATCH /api/matches/{id}/result`
- `POST /api/account/register`
- `POST /api/account/login`
- `POST /api/account/logout`

## Run

Open `FootballLeagueManagement.sln` in Visual Studio and start `FootballLeagueManagement.Web`, or run:

```bash
dotnet run --project FootballLeagueManagement.Web
```
