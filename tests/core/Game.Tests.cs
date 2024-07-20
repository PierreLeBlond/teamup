namespace Core.Tests
{
    public class GameTest
    {
        [Fact]
        public void ShouldCreateAGame()
        {
            Game game = new();
            Assert.NotNull(game);
        }
    }

    public class ShouldAddATeamTest
    {
        [Fact]
        public void ShouldAddATeam()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            Assert.Equal(team, game.Teams[0]);
        }
    }

    public class ShouldGetATeamTest
    {
        [Fact]
        public void ShouldGetATeam()
        {
            Game game = new();
            Team team = new();
            team.AddPlayer("Pierre");
            game.AddTeam(team);
            Assert.Equal(team, game.GetTeam("Pierre"));
        }

        [Fact]
        public void ShouldGetNullIfTeamDoesNotExists()
        {
            Game game = new();
            Assert.Null(game.GetTeam("Pierre"));
        }

        [Fact]
        public void ShouldGetNullIfNameDoesNotExists()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            Assert.Null(game.GetTeam("Pierre"));
        }
    }

    public class ShouldAddRewardTest
    {
        [Fact]
        public void ShouldAddAReward()
        {
            Game game = new();
            game.AddReward(20);
            game.AddReward(10);
        }
    }

    public class ShouldSetTeamRankTest
    {
        [Fact]
        public void ShouldSetTeamRank()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            game.SetTeamRank(team, 1);
        }

        [Fact]
        public void ShouldThrowIfTeamDoesNotExistsWithinGame()
        {
            Game game = new();
            Team team = new();
            Assert.Throws<ArgumentException>(() => game.SetTeamRank(team, 1));
        }

        [Fact]
        public void ShouldThrowWhenSettingAnOutOfBoundRank()
        {
            Game game = new();
            Team team = new();
            Assert.Throws<ArgumentException>(() => game.SetTeamRank(team, 2));
        }

        [Fact]
        public void ShouldThrowWhenSettingAnOutOfBoundRankWithAlreadyRankedTeam()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            game.SetTeamRank(team, 1);
            Assert.Throws<ArgumentException>(() => game.SetTeamRank(team, 2));
        }

        [Fact]
        public void ShouldMoveTeamsDownBelowGivenRank()
        {
            Game game = new();
            Team team1 = new();
            Team team2 = new();
            game.AddTeam(team1);
            game.AddTeam(team2);
            game.SetTeamRank(team1, 1);
            game.SetTeamRank(team2, 1);
            Assert.Equal(2, game.GetTeamRank(team1));
            Assert.Equal(1, game.GetTeamRank(team2));
        }

        [Fact]
        public void ShouldUpdateAlreadyRankedTeam()
        {
            Game game = new();
            Team team1 = new();
            Team team2 = new();
            game.AddTeam(team1);
            game.AddTeam(team2);
            game.SetTeamRank(team1, 1);
            game.SetTeamRank(team2, 2);
            game.SetTeamRank(team2, 1);
            Assert.Equal(2, game.GetTeamRank(team1));
            Assert.Equal(1, game.GetTeamRank(team2));
        }
    }

    public class ShouldGetRewardTest
    {
        [Fact]
        public void ShouldGetAReward()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            game.AddReward(20);
            game.AddReward(10);
            game.SetTeamRank(team, 1);
            Assert.Equal(20, game.GetReward(team));
        }

        [Fact]
        public void ShouldGetNullIfTeamIsNotRanked()
        {
            Game game = new();
            Team team = new();
            game.AddReward(20);
            game.AddReward(10);
            Assert.Null(game.GetReward(team));
        }
    }

    public class ShouldGetTeamRankTest
    {
        [Fact]
        public void ShouldGetTeamRank()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            game.SetTeamRank(team, 1);
            Assert.Equal(1, game.GetTeamRank(team));
        }

        [Fact]
        public void ShouldThrowIfTeamDoesNotExistsWithinGame()
        {
            Game game = new();
            Team team = new();
            Assert.Throws<ArgumentException>(() => game.GetTeamRank(team));
        }

        [Fact]
        public void ShouldGetNullIfTeamHasNoRank()
        {
            Game game = new();
            Team team = new();
            game.AddTeam(team);
            Assert.Null(game.GetTeamRank(team));
        }
    }
}
