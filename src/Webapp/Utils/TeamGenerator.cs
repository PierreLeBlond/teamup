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
    private static List<Player> GetLeastScoringTeam(List<List<Player>> teams)
    {
        if (teams.Count == 0)
        {
            throw new ArgumentException("teams must not be empty");
        }

        List<Player> leastScoringTeam = null!;
        var leastScore = int.MaxValue;
        foreach (var team in teams)
        {
            var score = team.Aggregate(0, (current, player) => current + player.Score);
            if (score < leastScore)
            {
                leastScore = score;
                leastScoringTeam = team;
            }
        }
        return leastScoringTeam;
    }

    public static List<List<Player>> GenerateTeams(List<Player> players, int numberOfTeams)
    {
        var teams = new List<List<Player>>();
        for (int i = 0; i < numberOfTeams; i++)
        {
            teams.Add([]);
        }
        var sortedPlayers = new List<Player>(players);
        sortedPlayers.Sort(new Comparer());

        foreach (var player in sortedPlayers)
        {
            var team = GetLeastScoringTeam(teams);
            team.Add(player);
        }

        return teams;
    }
}
