using FootballLeagueManagement.Core.Dtos;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Services;

public static class DemoLeagueData
{
    public static IReadOnlyList<Stadium> Stadiums { get; } =
    [
        new Stadium { Id = 1, Name = "Anfield", City = "Liverpool", Capacity = 61276 },
        new Stadium { Id = 2, Name = "Emirates Stadium", City = "London", Capacity = 60383 },
        new Stadium { Id = 3, Name = "Etihad Stadium", City = "Manchester", Capacity = 53400 },
        new Stadium { Id = 4, Name = "Old Trafford", City = "Manchester", Capacity = 74310 },
        new Stadium { Id = 5, Name = "Stamford Bridge", City = "London", Capacity = 41837 },
        new Stadium { Id = 6, Name = "Tottenham Hotspur Stadium", City = "London", Capacity = 62850 }
    ];

    public static IReadOnlyList<Club> Clubs { get; } =
    [
        new Club { Id = 1, Name = "Liverpool", ShortCode = "LIV", City = "Liverpool", FoundedYear = 1892, StadiumId = 1, Stadium = Stadiums[0] },
        new Club { Id = 2, Name = "Arsenal", ShortCode = "ARS", City = "London", FoundedYear = 1886, StadiumId = 2, Stadium = Stadiums[1] },
        new Club { Id = 3, Name = "Manchester City", ShortCode = "MCI", City = "Manchester", FoundedYear = 1880, StadiumId = 3, Stadium = Stadiums[2] },
        new Club { Id = 4, Name = "Manchester United", ShortCode = "MUN", City = "Manchester", FoundedYear = 1878, StadiumId = 4, Stadium = Stadiums[3] },
        new Club { Id = 5, Name = "Chelsea", ShortCode = "CHE", City = "London", FoundedYear = 1905, StadiumId = 5, Stadium = Stadiums[4] },
        new Club { Id = 6, Name = "Tottenham Hotspur", ShortCode = "TOT", City = "London", FoundedYear = 1882, StadiumId = 6, Stadium = Stadiums[5] }
    ];

    public static IReadOnlyList<Player> Players { get; } =
    [
        new Player { Id = 1, FullName = "Mohamed Salah", Position = "Forward", ShirtNumber = 11, ClubId = 1 },
        new Player { Id = 2, FullName = "Bukayo Saka", Position = "Forward", ShirtNumber = 7, ClubId = 2 },
        new Player { Id = 3, FullName = "Erling Haaland", Position = "Forward", ShirtNumber = 9, ClubId = 3 },
        new Player { Id = 4, FullName = "Bruno Fernandes", Position = "Midfielder", ShirtNumber = 8, ClubId = 4 },
        new Player { Id = 5, FullName = "Cole Palmer", Position = "Midfielder", ShirtNumber = 20, ClubId = 5 },
        new Player { Id = 6, FullName = "Son Heung-min", Position = "Forward", ShirtNumber = 7, ClubId = 6 }
    ];

    public static IReadOnlyList<Match> Matches { get; } =
    [
        new Match { Id = 1, HomeClubId = 1, HomeClub = Clubs[0], AwayClubId = 2, AwayClub = Clubs[1], KickoffUtc = new DateTime(2025, 9, 20, 15, 0, 0, DateTimeKind.Utc), HomeGoals = 2, AwayGoals = 1 },
        new Match { Id = 2, HomeClubId = 3, HomeClub = Clubs[2], AwayClubId = 5, AwayClub = Clubs[4], KickoffUtc = new DateTime(2025, 9, 21, 16, 30, 0, DateTimeKind.Utc), HomeGoals = 3, AwayGoals = 1 },
        new Match { Id = 3, HomeClubId = 6, HomeClub = Clubs[5], AwayClubId = 4, AwayClub = Clubs[3], KickoffUtc = new DateTime(2025, 9, 22, 19, 0, 0, DateTimeKind.Utc), HomeGoals = 1, AwayGoals = 1 },
        new Match { Id = 4, HomeClubId = 2, HomeClub = Clubs[1], AwayClubId = 3, AwayClub = Clubs[2], KickoffUtc = new DateTime(2025, 10, 4, 14, 0, 0, DateTimeKind.Utc) }
    ];

    public static LeagueSummaryDto Summary =>
        new(Clubs.Count, Players.Count, Stadiums.Count, Matches.Count, Matches.Count(match => match.IsPlayed), Standings.First().Club);

    public static IReadOnlyList<StandingDto> Standings
    {
        get
        {
            var rows = Clubs.ToDictionary(club => club.Id, club => new StandingAccumulator(club.Name));

            foreach (var match in Matches.Where(match => match.IsPlayed))
            {
                rows[match.HomeClubId].Apply(match.HomeGoals!.Value, match.AwayGoals!.Value);
                rows[match.AwayClubId].Apply(match.AwayGoals!.Value, match.HomeGoals!.Value);
            }

            return rows.Values
                .Select(row => row.ToDto())
                .OrderByDescending(row => row.Points)
                .ThenByDescending(row => row.GoalDifference)
                .ThenBy(row => row.Club)
                .Select((row, index) => row with { Position = index + 1 })
                .ToList();
        }
    }

    public static IReadOnlyList<Club> SearchClubs(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Clubs;
        }

        return Clubs
            .Where(club => club.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                           club.City.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static IReadOnlyList<Match> FilterMatches(bool? played)
    {
        return played switch
        {
            true => Matches.Where(match => match.IsPlayed).ToList(),
            false => Matches.Where(match => !match.IsPlayed).ToList(),
            _ => Matches
        };
    }

    public static IEnumerable<object> ClubResponses(string? search) =>
        SearchClubs(search).Select(club => new
        {
            club.Id,
            club.Name,
            club.ShortCode,
            club.City,
            club.FoundedYear,
            Stadium = new
            {
                club.Stadium!.Id,
                club.Stadium.Name,
                club.Stadium.City,
                club.Stadium.Capacity
            }
        });

    public static IEnumerable<object> PlayerResponses(int? clubId)
    {
        var players = Players.AsEnumerable();
        if (clubId.HasValue)
        {
            players = players.Where(player => player.ClubId == clubId.Value);
        }

        return players.Select(player => new
        {
            player.Id,
            player.FullName,
            player.Position,
            player.ShirtNumber,
            player.ClubId,
            Club = Clubs.First(club => club.Id == player.ClubId).Name
        });
    }

    public static IEnumerable<object> StadiumResponses() =>
        Stadiums.Select(stadium => new
        {
            stadium.Id,
            stadium.Name,
            stadium.City,
            stadium.Capacity
        });

    public static IEnumerable<object> MatchResponses(bool? played) =>
        FilterMatches(played).Select(match => new
        {
            match.Id,
            match.KickoffUtc,
            match.HomeGoals,
            match.AwayGoals,
            HomeClub = new
            {
                match.HomeClub!.Id,
                match.HomeClub.Name,
                match.HomeClub.ShortCode
            },
            AwayClub = new
            {
                match.AwayClub!.Id,
                match.AwayClub.Name,
                match.AwayClub.ShortCode
            }
        });

    private sealed class StandingAccumulator(string club)
    {
        private int _played;
        private int _wins;
        private int _draws;
        private int _losses;
        private int _goalsFor;
        private int _goalsAgainst;

        public void Apply(int goalsFor, int goalsAgainst)
        {
            _played++;
            _goalsFor += goalsFor;
            _goalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
            {
                _wins++;
            }
            else if (goalsFor == goalsAgainst)
            {
                _draws++;
            }
            else
            {
                _losses++;
            }
        }

        public StandingDto ToDto() =>
            new(0, club, _played, _wins, _draws, _losses, _goalsFor, _goalsAgainst, _goalsFor - _goalsAgainst, _wins * 3 + _draws);
    }
}
