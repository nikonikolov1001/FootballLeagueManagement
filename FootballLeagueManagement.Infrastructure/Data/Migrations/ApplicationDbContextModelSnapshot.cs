using FootballLeagueManagement.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace FootballLeagueManagement.Infrastructure.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.14");

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property<string>("Id");
            entity.Property<string>("FullName");
            entity.Property<string>("FavoriteClub");
            entity.HasKey("Id");
            entity.ToTable("AspNetUsers");
        });
    }
}
