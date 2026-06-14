using FootballLeagueManagement.Core.Data;
using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Core.Services;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Infrastructure.Identity;
using FootballLeagueManagement.Infrastructure.Services;
using FootballLeagueManagement.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseSqlServer(connectionString)
        .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")))
    .SetApplicationName("FootballLeagueManagement");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login";
});

builder.Services.AddScoped<ILeagueQueryService, LeagueQueryService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IClubAdminService, ClubAdminService>();
builder.Services.AddScoped<IPlayerAdminService, PlayerAdminService>();
builder.Services.AddScoped<IStadiumAdminService, StadiumAdminService>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await dbContext.Database.ExecuteSqlRawAsync("""
        IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'AspNetUsers', N'FullName') IS NULL
        BEGIN
            ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL CONSTRAINT [DF_AspNetUsers_FullName] DEFAULT N''
        END

        IF OBJECT_ID(N'[AspNetUsers]', N'U') IS NOT NULL AND COL_LENGTH(N'AspNetUsers', N'FavoriteClub') IS NULL
        BEGIN
            ALTER TABLE [AspNetUsers] ADD [FavoriteClub] nvarchar(max) NOT NULL CONSTRAINT [DF_AspNetUsers_FavoriteClub] DEFAULT N''
        END
        """);
    await EnsureLeagueTablesAsync(dbContext);

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Administrator"))
    {
        await roleManager.CreateAsync(new IdentityRole("Administrator"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    var admin = await userManager.FindByEmailAsync("admin@premierleague.local");
    if (admin is null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin@premierleague.local",
            Email = "admin@premierleague.local",
            EmailConfirmed = true,
            FullName = "Premier League Administrator",
            FavoriteClub = "Liverpool"
        };

        await userManager.CreateAsync(admin, "Admin123!");
    }

    if (!await userManager.IsInRoleAsync(admin, "Administrator"))
    {
        await userManager.AddToRoleAsync(admin, "Administrator");
    }

    if (!await userManager.CheckPasswordAsync(admin, "Admin123!"))
    {
        await userManager.RemovePasswordAsync(admin);
        await userManager.AddPasswordAsync(admin, "Admin123!");
    }

    await SeedLeagueAsync(dbContext);
}

static async Task EnsureLeagueTablesAsync(ApplicationDbContext dbContext)
{
    await dbContext.Database.ExecuteSqlRawAsync("""
        IF OBJECT_ID(N'[Stadiums]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Stadiums] (
                [Id] int NOT NULL IDENTITY(1,1),
                [Name] nvarchar(100) NOT NULL,
                [City] nvarchar(80) NOT NULL,
                [Capacity] int NOT NULL,
                CONSTRAINT [PK_Stadiums] PRIMARY KEY ([Id])
            )
        END

        IF OBJECT_ID(N'[Clubs]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Clubs] (
                [Id] int NOT NULL IDENTITY(1,1),
                [Name] nvarchar(80) NOT NULL,
                [ShortCode] nvarchar(8) NOT NULL,
                [City] nvarchar(80) NOT NULL,
                [FoundedYear] int NOT NULL,
                [StadiumId] int NOT NULL,
                CONSTRAINT [PK_Clubs] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_Clubs_Stadiums_StadiumId] FOREIGN KEY ([StadiumId]) REFERENCES [Stadiums] ([Id]) ON DELETE NO ACTION
            )
            CREATE UNIQUE INDEX [IX_Clubs_Name] ON [Clubs] ([Name])
        END

        IF OBJECT_ID(N'[Players]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Players] (
                [Id] int NOT NULL IDENTITY(1,1),
                [FullName] nvarchar(100) NOT NULL,
                [Position] nvarchar(40) NOT NULL,
                [ShirtNumber] int NOT NULL,
                [ClubId] int NOT NULL,
                CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_Players_Clubs_ClubId] FOREIGN KEY ([ClubId]) REFERENCES [Clubs] ([Id]) ON DELETE CASCADE
            )
            CREATE INDEX [IX_Players_ClubId] ON [Players] ([ClubId])
        END

        IF OBJECT_ID(N'[Matches]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Matches] (
                [Id] int NOT NULL IDENTITY(1,1),
                [HomeClubId] int NOT NULL,
                [AwayClubId] int NOT NULL,
                [KickoffUtc] datetime2 NOT NULL,
                [HomeGoals] int NULL,
                [AwayGoals] int NULL,
                CONSTRAINT [PK_Matches] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_Matches_Clubs_AwayClubId] FOREIGN KEY ([AwayClubId]) REFERENCES [Clubs] ([Id]) ON DELETE NO ACTION,
                CONSTRAINT [FK_Matches_Clubs_HomeClubId] FOREIGN KEY ([HomeClubId]) REFERENCES [Clubs] ([Id]) ON DELETE NO ACTION
            )
            CREATE INDEX [IX_Matches_AwayClubId] ON [Matches] ([AwayClubId])
            CREATE INDEX [IX_Matches_HomeClubId] ON [Matches] ([HomeClubId])
        END

        IF COL_LENGTH(N'Stadiums', N'Name') IS NULL
            ALTER TABLE [Stadiums] ADD [Name] nvarchar(100) NOT NULL CONSTRAINT [DF_Stadiums_Name] DEFAULT N''
        IF COL_LENGTH(N'Stadiums', N'City') IS NULL
            ALTER TABLE [Stadiums] ADD [City] nvarchar(80) NOT NULL CONSTRAINT [DF_Stadiums_City] DEFAULT N''
        IF COL_LENGTH(N'Stadiums', N'Capacity') IS NULL
            ALTER TABLE [Stadiums] ADD [Capacity] int NOT NULL CONSTRAINT [DF_Stadiums_Capacity] DEFAULT 1000

        IF COL_LENGTH(N'Clubs', N'Name') IS NULL
            ALTER TABLE [Clubs] ADD [Name] nvarchar(80) NOT NULL CONSTRAINT [DF_Clubs_Name] DEFAULT N''
        IF COL_LENGTH(N'Clubs', N'ShortCode') IS NULL
            ALTER TABLE [Clubs] ADD [ShortCode] nvarchar(8) NOT NULL CONSTRAINT [DF_Clubs_ShortCode] DEFAULT N''
        IF COL_LENGTH(N'Clubs', N'City') IS NULL
            ALTER TABLE [Clubs] ADD [City] nvarchar(80) NOT NULL CONSTRAINT [DF_Clubs_City] DEFAULT N''
        IF COL_LENGTH(N'Clubs', N'FoundedYear') IS NULL
            ALTER TABLE [Clubs] ADD [FoundedYear] int NOT NULL CONSTRAINT [DF_Clubs_FoundedYear] DEFAULT 1900
        IF COL_LENGTH(N'Clubs', N'StadiumId') IS NULL
            ALTER TABLE [Clubs] ADD [StadiumId] int NOT NULL CONSTRAINT [DF_Clubs_StadiumId] DEFAULT 1

        IF COL_LENGTH(N'Players', N'FullName') IS NULL
            ALTER TABLE [Players] ADD [FullName] nvarchar(100) NOT NULL CONSTRAINT [DF_Players_FullName] DEFAULT N''
        IF COL_LENGTH(N'Players', N'Position') IS NULL
            ALTER TABLE [Players] ADD [Position] nvarchar(40) NOT NULL CONSTRAINT [DF_Players_Position] DEFAULT N''
        IF COL_LENGTH(N'Players', N'ShirtNumber') IS NULL
            ALTER TABLE [Players] ADD [ShirtNumber] int NOT NULL CONSTRAINT [DF_Players_ShirtNumber] DEFAULT 1
        IF COL_LENGTH(N'Players', N'ClubId') IS NULL
            ALTER TABLE [Players] ADD [ClubId] int NOT NULL CONSTRAINT [DF_Players_ClubId] DEFAULT 1

        IF COL_LENGTH(N'Matches', N'HomeClubId') IS NULL
            ALTER TABLE [Matches] ADD [HomeClubId] int NOT NULL CONSTRAINT [DF_Matches_HomeClubId] DEFAULT 1
        IF COL_LENGTH(N'Matches', N'AwayClubId') IS NULL
            ALTER TABLE [Matches] ADD [AwayClubId] int NOT NULL CONSTRAINT [DF_Matches_AwayClubId] DEFAULT 2
        IF COL_LENGTH(N'Matches', N'KickoffUtc') IS NULL
            ALTER TABLE [Matches] ADD [KickoffUtc] datetime2 NOT NULL CONSTRAINT [DF_Matches_KickoffUtc] DEFAULT SYSUTCDATETIME()
        IF COL_LENGTH(N'Matches', N'HomeGoals') IS NULL
            ALTER TABLE [Matches] ADD [HomeGoals] int NULL
        IF COL_LENGTH(N'Matches', N'AwayGoals') IS NULL
            ALTER TABLE [Matches] ADD [AwayGoals] int NULL
        """);
}

static async Task SeedLeagueAsync(ApplicationDbContext dbContext)
{
    await dbContext.Database.OpenConnectionAsync();
    try
    {
        var existingStadiumIds = (await dbContext.Stadiums.Select(stadium => stadium.Id).ToListAsync()).ToHashSet();
        var missingStadiums = PremierLeagueSnapshot.Stadiums
            .Where(stadium => !existingStadiumIds.Contains(stadium.Id))
            .ToList();
        if (missingStadiums.Count > 0)
        {
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Stadiums] ON");
            dbContext.Stadiums.AddRange(missingStadiums.Select(stadium => new Stadium
            {
                Id = stadium.Id,
                Name = stadium.Name,
                City = stadium.City,
                Capacity = stadium.Capacity
            }));

            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Stadiums] OFF");
        }

        var existingClubIds = (await dbContext.Clubs.Select(club => club.Id).ToListAsync()).ToHashSet();
        var missingClubs = PremierLeagueSnapshot.Clubs
            .Where(club => !existingClubIds.Contains(club.Id))
            .ToList();
        if (missingClubs.Count > 0)
        {
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Clubs] ON");
            dbContext.Clubs.AddRange(missingClubs.Select(club => new Club
            {
                Id = club.Id,
                Name = club.Name,
                ShortCode = club.ShortCode,
                City = club.City,
                FoundedYear = club.FoundedYear,
                StadiumId = club.StadiumId
            }));

            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Clubs] OFF");
        }

        var existingPlayerIds = (await dbContext.Players.Select(player => player.Id).ToListAsync()).ToHashSet();
        var missingPlayers = PremierLeagueSnapshot.Players
            .Where(player => !existingPlayerIds.Contains(player.Id))
            .ToList();
        if (missingPlayers.Count > 0)
        {
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Players] ON");
            dbContext.Players.AddRange(missingPlayers.Select(player => new Player
            {
                Id = player.Id,
                FullName = player.FullName,
                Position = player.Position,
                ShirtNumber = player.ShirtNumber,
                ClubId = player.ClubId
            }));

            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Players] OFF");
        }

        var existingMatchIds = (await dbContext.Matches.Select(match => match.Id).ToListAsync()).ToHashSet();
        var missingMatches = PremierLeagueSnapshot.Matches
            .Where(match => !existingMatchIds.Contains(match.Id))
            .ToList();
        if (missingMatches.Count > 0)
        {
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Matches] ON");
            dbContext.Matches.AddRange(missingMatches.Select(match => new Match
            {
                Id = match.Id,
                HomeClubId = match.HomeClubId,
                AwayClubId = match.AwayClubId,
                KickoffUtc = match.KickoffUtc,
                HomeGoals = match.HomeGoals,
                AwayGoals = match.AwayGoals
            }));

            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Matches] OFF");
        }
    }
    finally
    {
        await dbContext.Database.CloseConnectionAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
