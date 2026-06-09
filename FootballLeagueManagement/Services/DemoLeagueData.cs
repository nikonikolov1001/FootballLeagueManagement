using FootballLeagueManagement.Core.Data;
using FootballLeagueManagement.Core.Dtos;
using FootballLeagueManagement.Core.Models;

namespace FootballLeagueManagement.Services;

public static class DemoLeagueData
{
    public static IReadOnlyList<Stadium> Stadiums { get; } =
        PremierLeagueSnapshot.Stadiums
            .Select(stadium => new Stadium
            {
                Id = stadium.Id,
                Name = stadium.Name,
                City = stadium.City,
                Capacity = stadium.Capacity
            })
            .ToList();

    public static IReadOnlyList<Club> Clubs { get; } =
        PremierLeagueSnapshot.Clubs
            .Select(club => new Club
            {
                Id = club.Id,
                Name = club.Name,
                ShortCode = club.ShortCode,
                City = club.City,
                FoundedYear = club.FoundedYear,
                StadiumId = club.StadiumId,
                Stadium = Stadiums.First(stadium => stadium.Id == club.StadiumId)
            })
            .ToList();

    public static IReadOnlyList<Player> Players { get; } =
        PremierLeagueSnapshot.Players
            .Select(player => new Player
            {
                Id = player.Id,
                FullName = player.FullName,
                Position = player.Position,
                ShirtNumber = player.ShirtNumber,
                ClubId = player.ClubId,
                Club = Clubs.First(club => club.Id == player.ClubId)
            })
            .ToList();

    public static IReadOnlyList<Match> Matches { get; } =
        PremierLeagueSnapshot.Matches
            .Select(match => new Match
            {
                Id = match.Id,
                HomeClubId = match.HomeClubId,
                HomeClub = Clubs.First(club => club.Id == match.HomeClubId),
                AwayClubId = match.AwayClubId,
                AwayClub = Clubs.First(club => club.Id == match.AwayClubId),
                KickoffUtc = match.KickoffUtc,
                HomeGoals = match.HomeGoals,
                AwayGoals = match.AwayGoals
            })
            .ToList();

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
            Club = Clubs.First(club => club.Id == player.ClubId).Name,
            Stats = BuildPlayerStats(player.FullName)
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

    public static object BuildPlayerStats(string fullName)
    {
        var player = PremierLeagueSnapshot.Players.FirstOrDefault(player => player.FullName == fullName);

        return new
        {
            Appearances = player?.Appearances ?? 0,
            Goals = player?.Goals ?? 0,
            Assists = player?.Assists ?? 0,
            Rating = player?.Rating ?? 0
        };
    }
}
