 namespace FootballLeagueManagement.Core.Data;

public static class PremierLeagueSnapshot
{
    private const int PlayedRounds = 36;

    public static IReadOnlyList<StadiumSeed> Stadiums { get; } =
    [
        new(1, "Anfield", "Liverpool", 61276),
        new(2, "Emirates Stadium", "London", 60383),
        new(3, "Etihad Stadium", "Manchester", 53400),
        new(4, "Stamford Bridge", "London", 41837),
        new(5, "St James' Park", "Newcastle upon Tyne", 52305),
        new(6, "Tottenham Hotspur Stadium", "London", 62850),
        new(7, "Villa Park", "Birmingham", 42657),
        new(8, "Old Trafford", "Manchester", 74310),
        new(9, "Amex Stadium", "Brighton", 31800),
        new(10, "Vitality Stadium", "Bournemouth", 11379),
        new(11, "Gtech Community Stadium", "London", 17250),
        new(12, "Craven Cottage", "London", 25700),
        new(13, "Selhurst Park", "London", 25486),
        new(14, "Goodison Park", "Liverpool", 39572),
        new(15, "London Stadium", "London", 62500),
        new(16, "Molineux Stadium", "Wolverhampton", 31750),
        new(17, "City Ground", "Nottingham", 30445),
        new(18, "Elland Road", "Leeds", 37890),
        new(19, "Turf Moor", "Burnley", 21944),
        new(20, "Stadium of Light", "Sunderland", 48707)
    ];

    public static IReadOnlyList<ClubSeed> Clubs { get; } =
    [
        new(1, "Liverpool", "LIV", "Liverpool", 1892, 1, 20),
        new(2, "Arsenal", "ARS", "London", 1886, 2, 19),
        new(3, "Manchester City", "MCI", "Manchester", 1880, 3, 18),
        new(4, "Chelsea", "CHE", "London", 1905, 4, 16),
        new(5, "Newcastle United", "NEW", "Newcastle upon Tyne", 1892, 5, 15),
        new(6, "Tottenham Hotspur", "TOT", "London", 1882, 6, 14),
        new(7, "Aston Villa", "AVL", "Birmingham", 1874, 7, 13),
        new(8, "Manchester United", "MUN", "Manchester", 1878, 8, 12),
        new(9, "Brighton & Hove Albion", "BHA", "Brighton", 1901, 9, 11),
        new(10, "AFC Bournemouth", "BOU", "Bournemouth", 1899, 10, 10),
        new(11, "Brentford", "BRE", "London", 1889, 11, 9),
        new(12, "Fulham", "FUL", "London", 1879, 12, 8),
        new(13, "Crystal Palace", "CRY", "London", 1905, 13, 8),
        new(14, "Everton", "EVE", "Liverpool", 1878, 14, 7),
        new(15, "West Ham United", "WHU", "London", 1895, 15, 7),
        new(16, "Wolverhampton Wanderers", "WOL", "Wolverhampton", 1877, 16, 6),
        new(17, "Nottingham Forest", "NFO", "Nottingham", 1865, 17, 6),
        new(18, "Leeds United", "LEE", "Leeds", 1919, 18, 5),
        new(19, "Burnley", "BUR", "Burnley", 1882, 19, 4),
        new(20, "Sunderland", "SUN", "Sunderland", 1879, 20, 4)
    ];

    public static IReadOnlyList<PlayerSeed> Players { get; } =
    [
        new(1, "Mohamed Salah", "Forward", 11, 1, 34, 24, 12, 8.3m),
        new(2, "Virgil van Dijk", "Defender", 4, 1, 35, 3, 1, 7.7m),
        new(3, "Bukayo Saka", "Forward", 7, 2, 33, 18, 11, 8.0m),
        new(4, "Declan Rice", "Midfielder", 41, 2, 35, 6, 8, 7.6m),
        new(5, "Erling Haaland", "Forward", 9, 3, 32, 27, 5, 8.1m),
        new(6, "Phil Foden", "Midfielder", 47, 3, 34, 14, 10, 7.8m),
        new(7, "Cole Palmer", "Midfielder", 20, 4, 35, 19, 13, 8.2m),
        new(8, "Enzo Fernandez", "Midfielder", 8, 4, 31, 5, 9, 7.4m),
        new(9, "Bruno Guimaraes", "Midfielder", 39, 5, 34, 7, 8, 7.5m),
        new(10, "Alexander Isak", "Forward", 14, 5, 31, 22, 4, 7.9m),
        new(11, "Son Heung-min", "Forward", 7, 6, 33, 17, 10, 7.7m),
        new(12, "James Maddison", "Midfielder", 10, 6, 29, 8, 12, 7.5m),
        new(13, "Ollie Watkins", "Forward", 11, 7, 35, 20, 9, 7.8m),
        new(14, "Emiliano Martinez", "Goalkeeper", 1, 7, 35, 0, 0, 7.3m),
        new(15, "Bruno Fernandes", "Midfielder", 8, 8, 35, 11, 12, 7.6m),
        new(16, "Kobbie Mainoo", "Midfielder", 37, 8, 30, 4, 5, 7.1m),
        new(17, "Kaoru Mitoma", "Forward", 22, 9, 30, 9, 8, 7.2m),
        new(18, "Lewis Dunk", "Defender", 5, 9, 34, 2, 2, 7.0m),
        new(19, "Dominic Solanke", "Forward", 9, 10, 35, 18, 6, 7.4m),
        new(20, "Marcus Tavernier", "Midfielder", 16, 10, 32, 6, 7, 7.0m),
        new(21, "Bryan Mbeumo", "Forward", 19, 11, 31, 13, 8, 7.3m),
        new(22, "Yoane Wissa", "Forward", 11, 11, 32, 12, 5, 7.1m),
        new(23, "Andreas Pereira", "Midfielder", 18, 12, 34, 5, 9, 7.1m),
        new(24, "Bernd Leno", "Goalkeeper", 17, 12, 35, 0, 0, 7.2m),
        new(25, "Eberechi Eze", "Midfielder", 10, 13, 30, 10, 7, 7.4m),
        new(26, "Marc Guehi", "Defender", 6, 13, 31, 1, 1, 7.0m),
        new(27, "Jordan Pickford", "Goalkeeper", 1, 14, 35, 0, 1, 7.1m),
        new(28, "James Tarkowski", "Defender", 6, 14, 35, 2, 1, 7.0m),
        new(29, "Jarrod Bowen", "Forward", 20, 15, 34, 15, 7, 7.4m),
        new(30, "Lucas Paqueta", "Midfielder", 10, 15, 31, 6, 7, 7.2m),
        new(31, "Matheus Cunha", "Forward", 12, 16, 32, 14, 6, 7.3m),
        new(32, "Joao Gomes", "Midfielder", 8, 16, 33, 3, 5, 6.9m),
        new(33, "Morgan Gibbs-White", "Midfielder", 10, 17, 34, 8, 10, 7.3m),
        new(34, "Taiwo Awoniyi", "Forward", 9, 17, 28, 9, 3, 6.9m),
        new(35, "Pascal Struijk", "Defender", 21, 18, 33, 2, 2, 6.8m),
        new(36, "Daniel James", "Forward", 7, 18, 34, 7, 8, 6.9m),
        new(37, "Josh Brownhill", "Midfielder", 8, 19, 35, 5, 5, 6.8m),
        new(38, "Lyle Foster", "Forward", 17, 19, 30, 8, 3, 6.8m),
        new(39, "Dan Neil", "Midfielder", 24, 20, 35, 4, 6, 6.8m),
        new(40, "Jack Clarke", "Forward", 20, 20, 34, 10, 7, 7.0m)
    ];

    public static IReadOnlyList<MatchSeed> Matches { get; } = BuildMatches();

    private static List<MatchSeed> BuildMatches()
    {
        var rounds = BuildRoundRobinRounds();
        var matches = new List<MatchSeed>(380);
        var matchId = 1;

        for (var roundIndex = 0; roundIndex < rounds.Count; roundIndex++)
        {
            var roundNumber = roundIndex + 1;
            var kickoff = new DateTime(2025, 8, 16, 14, 0, 0, DateTimeKind.Utc).AddDays(roundIndex * 7);
            AddRound(matches, rounds[roundIndex], roundNumber, kickoff, ref matchId);
        }

        for (var roundIndex = 0; roundIndex < rounds.Count; roundIndex++)
        {
            var roundNumber = roundIndex + 20;
            var kickoff = new DateTime(2026, 1, 10, 14, 0, 0, DateTimeKind.Utc).AddDays(roundIndex * 7);
            var reversedRound = rounds[roundIndex]
                .Select(match => (HomeClubId: match.AwayClubId, AwayClubId: match.HomeClubId))
                .ToList();
            AddRound(matches, reversedRound, roundNumber, kickoff, ref matchId);
        }

        return matches;
    }

    private static void AddRound(
        List<MatchSeed> matches,
        IReadOnlyList<(int HomeClubId, int AwayClubId)> round,
        int roundNumber,
        DateTime kickoff,
        ref int matchId)
    {
        foreach (var match in round)
        {
            var score = roundNumber <= PlayedRounds
                ? CalculateScore(match.HomeClubId, match.AwayClubId, roundNumber)
                : (HomeGoals: (int?)null, AwayGoals: (int?)null);

            matches.Add(new MatchSeed(
                matchId++,
                match.HomeClubId,
                match.AwayClubId,
                kickoff.AddHours(matches.Count % 5),
                score.HomeGoals,
                score.AwayGoals));
        }
    }

    private static List<List<(int HomeClubId, int AwayClubId)>> BuildRoundRobinRounds()
    {
        var teams = Clubs.Select(club => club.Id).ToList();
        var rounds = new List<List<(int HomeClubId, int AwayClubId)>>();

        for (var round = 0; round < teams.Count - 1; round++)
        {
            var fixtures = new List<(int HomeClubId, int AwayClubId)>();

            for (var index = 0; index < teams.Count / 2; index++)
            {
                var first = teams[index];
                var second = teams[teams.Count - 1 - index];
                fixtures.Add((HomeClubId: round % 2 == 0 ? first : second, AwayClubId: round % 2 == 0 ? second : first));
            }

            rounds.Add(fixtures);
            var last = teams[^1];
            teams.RemoveAt(teams.Count - 1);
            teams.Insert(1, last);
        }

        return rounds;
    }

    private static (int? HomeGoals, int? AwayGoals) CalculateScore(int homeClubId, int awayClubId, int roundNumber)
    {
        var homeStrength = Clubs.First(club => club.Id == homeClubId).Strength;
        var awayStrength = Clubs.First(club => club.Id == awayClubId).Strength;
        var ratingDiff = homeStrength - awayStrength + 2;
        var variation = PositiveModulo(homeClubId * 3 + awayClubId + roundNumber, 3) - 1;

        return (ratingDiff + variation) switch
        {
            >= 9 => (3, 0),
            >= 5 => (2, 0),
            >= 2 => (2, 1),
            >= 0 => (1, 1),
            >= -4 => (1, 2),
            _ => (0, 3)
        };
    }

    private static int PositiveModulo(int value, int divisor) => (value % divisor + divisor) % divisor;
}

public sealed record StadiumSeed(int Id, string Name, string City, int Capacity);

public sealed record ClubSeed(int Id, string Name, string ShortCode, string City, int FoundedYear, int StadiumId, int Strength);

public sealed record PlayerSeed(int Id, string FullName, string Position, int ShirtNumber, int ClubId, int Appearances, int Goals, int Assists, decimal Rating);

public sealed record MatchSeed(int Id, int HomeClubId, int AwayClubId, DateTime KickoffUtc, int? HomeGoals, int? AwayGoals);
