using Webapp.Models;

namespace Webapp.Utils;

class Comparer : IComparer<Player>
{
    public int Compare(Player? p1, Player? p2)
    {
        if (p1 == null)
        {
            return -1;
        }

        if (p2 == null)
        {
            return 1;
        }

        var compare = p2.Score.CompareTo(p1.Score);
        if (compare != 0)
        {
            return compare;
        }
        return p1.Name.CompareTo(p2.Name);
    }
}

public class TeamGenerator
{
    private static bool IsNextTeamLeast(
        List<Player>? currentTeam,
        int currentScore,
        List<Player> nextTeam,
        int nextScore,
        int maxTeamSize
    )
    {
        if (currentTeam == null)
        {
            return true;
        }

        if (nextTeam.Count == maxTeamSize)
        {
            return false;
        }

        if (nextScore > currentScore)
        {
            return false;
        }

        if (nextScore == currentScore && nextTeam.Count >= currentTeam.Count)
        {
            return false;
        }

        return true;
    }

    private static List<Player>? GetLeastScoringTeam(List<List<Player>> teams, int maxTeamSize)
    {
        List<Player>? leastScoringTeam = null;
        var leastScore = int.MaxValue;
        foreach (var nextTeam in teams)
        {
            var nextScore = nextTeam.Aggregate(0, (current, player) => current + player.Score);
            if (IsNextTeamLeast(leastScoringTeam, leastScore, nextTeam, nextScore, maxTeamSize))
            {
                leastScore = nextScore;
                leastScoringTeam = nextTeam;
            }
        }
        return leastScoringTeam;
    }

    public static List<List<Player>> GenerateTeams(List<Player> players, int numberOfTeams)
    {
        var maxTeamSize = (int)Math.Ceiling((float)players.Count / numberOfTeams);
        var teams = new List<List<Player>>();
        for (int i = 0; i < numberOfTeams; i++)
        {
            teams.Add([]);
        }
        var sortedPlayers = new List<Player>(players);
        sortedPlayers.Sort(new Comparer());

        foreach (var player in sortedPlayers)
        {
            var team =
                GetLeastScoringTeam(teams, maxTeamSize)
                ?? throw new Exception("Couldn't fond a least scoring team");
            team.Add(player);
        }

        return teams;
    }
}
