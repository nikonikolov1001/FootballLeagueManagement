using FootballLeagueManagement.Core.Data;
using FootballLeagueManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballLeagueManagement.Infrastructure.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260608210000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FavoriteClub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Stadiums",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Capacity = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Stadiums", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Clubs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                ShortCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                FoundedYear = table.Column<int>(type: "int", nullable: false),
                StadiumId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clubs", x => x.Id);
                table.ForeignKey("FK_Clubs_Stadiums_StadiumId", x => x.StadiumId, "Stadiums", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Matches",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                HomeClubId = table.Column<int>(type: "int", nullable: false),
                AwayClubId = table.Column<int>(type: "int", nullable: false),
                KickoffUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                HomeGoals = table.Column<int>(type: "int", nullable: true),
                AwayGoals = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Matches", x => x.Id);
                table.ForeignKey("FK_Matches_Clubs_AwayClubId", x => x.AwayClubId, "Clubs", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Matches_Clubs_HomeClubId", x => x.HomeClubId, "Clubs", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Players",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Position = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                ShirtNumber = table.Column<int>(type: "int", nullable: false),
                ClubId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Players", x => x.Id);
                table.ForeignKey("FK_Players_Clubs_ClubId", x => x.ClubId, "Clubs", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData("AspNetRoles", new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" }, new object[,]
        {
            { "role-admin", "Administrator", "ADMINISTRATOR", null },
            { "role-user", "User", "USER", null }
        });

        migrationBuilder.InsertData("Stadiums", new[] { "Id", "Name", "City", "Capacity" }, BuildStadiumSeed());
        migrationBuilder.InsertData("Clubs", new[] { "Id", "Name", "ShortCode", "City", "FoundedYear", "StadiumId" }, BuildClubSeed());
        migrationBuilder.InsertData("Players", new[] { "Id", "FullName", "Position", "ShirtNumber", "ClubId" }, BuildPlayerSeed());
        migrationBuilder.InsertData("Matches", new[] { "Id", "HomeClubId", "AwayClubId", "KickoffUtc", "HomeGoals", "AwayGoals" }, BuildMatchSeed());

        migrationBuilder.CreateIndex("IX_AspNetRoles_NormalizedName", "AspNetRoles", "NormalizedName", unique: true, filter: "[NormalizedName] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_AspNetUsers_NormalizedEmail", "AspNetUsers", "NormalizedEmail");
        migrationBuilder.CreateIndex("IX_AspNetUsers_NormalizedUserName", "AspNetUsers", "NormalizedUserName", unique: true, filter: "[NormalizedUserName] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
        migrationBuilder.CreateIndex("IX_Clubs_Name", "Clubs", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_Clubs_StadiumId", "Clubs", "StadiumId", unique: true);
        migrationBuilder.CreateIndex("IX_Matches_AwayClubId", "Matches", "AwayClubId");
        migrationBuilder.CreateIndex("IX_Matches_HomeClubId", "Matches", "HomeClubId");
        migrationBuilder.CreateIndex("IX_Players_ClubId", "Players", "ClubId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("AspNetUserRoles");
        migrationBuilder.DropTable("Matches");
        migrationBuilder.DropTable("Players");
        migrationBuilder.DropTable("AspNetRoles");
        migrationBuilder.DropTable("AspNetUsers");
        migrationBuilder.DropTable("Clubs");
        migrationBuilder.DropTable("Stadiums");
    }

    private static object[,] BuildStadiumSeed()
    {
        var rows = PremierLeagueSnapshot.Stadiums;
        var values = new object[rows.Count, 4];

        for (var index = 0; index < rows.Count; index++)
        {
            values[index, 0] = rows[index].Id;
            values[index, 1] = rows[index].Name;
            values[index, 2] = rows[index].City;
            values[index, 3] = rows[index].Capacity;
        }

        return values;
    }

    private static object[,] BuildClubSeed()
    {
        var rows = PremierLeagueSnapshot.Clubs;
        var values = new object[rows.Count, 6];

        for (var index = 0; index < rows.Count; index++)
        {
            values[index, 0] = rows[index].Id;
            values[index, 1] = rows[index].Name;
            values[index, 2] = rows[index].ShortCode;
            values[index, 3] = rows[index].City;
            values[index, 4] = rows[index].FoundedYear;
            values[index, 5] = rows[index].StadiumId;
        }

        return values;
    }

    private static object[,] BuildPlayerSeed()
    {
        var rows = PremierLeagueSnapshot.Players;
        var values = new object[rows.Count, 5];

        for (var index = 0; index < rows.Count; index++)
        {
            values[index, 0] = rows[index].Id;
            values[index, 1] = rows[index].FullName;
            values[index, 2] = rows[index].Position;
            values[index, 3] = rows[index].ShirtNumber;
            values[index, 4] = rows[index].ClubId;
        }

        return values;
    }

    private static object[,] BuildMatchSeed()
    {
        var rows = PremierLeagueSnapshot.Matches;
        var values = new object[rows.Count, 6];

        for (var index = 0; index < rows.Count; index++)
        {
            values[index, 0] = rows[index].Id;
            values[index, 1] = rows[index].HomeClubId;
            values[index, 2] = rows[index].AwayClubId;
            values[index, 3] = rows[index].KickoffUtc;
            values[index, 4] = rows[index].HomeGoals;
            values[index, 5] = rows[index].AwayGoals;
        }

        return values;
    }
}
