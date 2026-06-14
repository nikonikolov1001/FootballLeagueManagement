using FootballLeagueManagement.Core.Models;
using FootballLeagueManagement.Infrastructure.Data;
using FootballLeagueManagement.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagement.Services;

public class ClubAdminService(ApplicationDbContext dbContext) : IClubAdminService
{
    public async Task<AdminOperationResult<Club>> CreateAsync(ClubInputModel input, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(input, null, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        var club = new Club
        {
            Name = input.Name,
            ShortCode = input.ShortCode,
            City = input.City,
            FoundedYear = input.FoundedYear,
            StadiumId = input.StadiumId
        };

        dbContext.Clubs.Add(club);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Club>.Success(club);
    }

    public async Task<AdminOperationResult<Club>> UpdateAsync(int id, ClubInputModel input, CancellationToken cancellationToken = default)
    {
        var club = await dbContext.Clubs.FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        if (club is null)
        {
            return AdminOperationResult<Club>.NotFound();
        }

        var validation = await ValidateAsync(input, id, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        club.Name = input.Name;
        club.ShortCode = input.ShortCode;
        club.City = input.City;
        club.FoundedYear = input.FoundedYear;
        club.StadiumId = input.StadiumId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Club>.Success(club);
    }

    public async Task<AdminOperationResult<Club>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var club = await dbContext.Clubs.FirstOrDefaultAsync(club => club.Id == id, cancellationToken);
        if (club is null)
        {
            return AdminOperationResult<Club>.NotFound();
        }

        dbContext.Clubs.Remove(club);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminOperationResult<Club>.Success(club);
    }

    private async Task<AdminOperationResult<Club>?> ValidateAsync(ClubInputModel input, int? currentClubId, CancellationToken cancellationToken)
    {
        if (!await dbContext.Stadiums.AnyAsync(stadium => stadium.Id == input.StadiumId, cancellationToken))
        {
            return AdminOperationResult<Club>.BadRequest("The selected stadium does not exist.");
        }

        var duplicateExists = await dbContext.Clubs.AnyAsync(
            club => club.Id != currentClubId && (club.Name == input.Name || club.ShortCode == input.ShortCode),
            cancellationToken);

        return duplicateExists
            ? AdminOperationResult<Club>.Conflict("A club with the same name or short code already exists.")
            : null;
    }
}
