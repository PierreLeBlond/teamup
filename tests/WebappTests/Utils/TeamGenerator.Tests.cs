using Webapp.Models;
using Webapp.Utils;

namespace Webapp.Tests.Utils;

public class TeamGeneratorTest
{
    [Fact]
    public void ShoudGenerateTeams()
    {
        var players = new List<Player>();
        for (int i = 0; i < 50; i++)
        {
            players.Add(new Player { Name = $"player{i + 1}", Score = 50 - i });
        }
        var teams = TeamGenerator.GenerateTeams(players, 10);

        var expectedTeams = new List<List<Player>>();
        for (int i = 0; i < 10; i++)
        {
            expectedTeams.Add([]);
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var index = i % 2 == 0 ? j : 9 - j;
                expectedTeams.ElementAt(index).Add(players.ElementAt(i * 10 + j));
            }
        }

        Assert.Equal(expectedTeams, teams);
    }

    [Fact]
    public void ShoulGenerateTeamsWhenPlayersHaveNoScoreYet()
    {
        var players = new List<Player>();
        for (int i = 0; i < 4; i++)
        {
            players.Add(new Player { Name = $"player{i + 1}", Score = 0 });
        }
        var teams = TeamGenerator.GenerateTeams(players, 2);

        var expectedTeams = new List<List<Player>>
        {
            new List<Player> { players.ElementAt(0), players.ElementAt(2) },
            new List<Player> { players.ElementAt(1), players.ElementAt(3) },
        };

        Assert.Equal(expectedTeams, teams);
    }
}
