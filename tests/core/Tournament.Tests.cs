namespace Core.Tests
{
    public class TournamentTest
    {
        public static Tournament GetValidTournament()
        {
            Tournament tournament = new();
            tournament.AddNames(TournamentAddNamesTest.GetNames());
            return tournament;
        }

        [Fact]
        public void ShouldCreateATournament()
        {
            Tournament tournament = GetValidTournament();
            Assert.NotNull(tournament);
        }
    }

    public class TournamentAddNamesTest
    {
        public static string[] GetNames()
        {
            return ["Grincheux", "Simplet", "Atchoum", "Prof", "Dormeur", "Joyeux", "Timide"];
        }

        [Fact]
        public void ShouldAddNames()
        {
            Tournament tournament = new();
            tournament.AddNames(GetNames());

            Assert.Equivalent(GetNames(), tournament.Names);
        }

        [Fact]
        public void ShouldThrowIfANameIsEmpty()
        {
            Tournament tournament = new();
            Assert.Throws<ArgumentException>(() => tournament.AddNames([""]));
        }
    }

    public class TournamentCreateGameTest
    {
        public static Game GetValidGame(Tournament tournament)
        {
            return tournament.CreateGame([100, 50]);
        }

        [Fact]
        public void ShouldCreateGame()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Game game = GetValidGame(tournament);

            Assert.Equal(game, tournament.Games[0]);
        }

        [Fact]
        public void ShouldThrowIfLessThanTwoNames()
        {
            Tournament tournament = new();
            tournament.AddNames(["Pierre"]);
            Assert.Throws<InvalidOperationException>(() => GetValidGame(tournament));
        }

        [Fact]
        public void ShouldThrowIfLessThanTwoTeams()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Assert.Throws<ArgumentException>(() => tournament.CreateGame([100]));
        }

        [Fact]
        public void ShouldCreateTeamsWithABalancedNumberOfPlayers()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Game game = GetValidGame(tournament);
            Assert.Equal(4, game.Teams[0].Players.Length);
            Assert.Equal(3, game.Teams[1].Players.Length);
        }

        [Fact]
        public void ShouldCreateFairSecondGameTeams()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Game game = GetValidGame(tournament);

            game.SetTeamRank(game.Teams[0], 1);
            game.SetTeamRank(game.Teams[1], 2);

            game = GetValidGame(tournament);

            double score1 = game.Teams[0].Players.Sum(player => tournament.GetScore(player.Name));
            double score2 = game.Teams[1].Players.Sum(player => tournament.GetScore(player.Name));

            Assert.InRange(score1 - score2, -50, 50);
        }
    }

    public class TournamentGetScoreTest
    {
        static Tournament GetFinishedTournament()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Game game = TournamentCreateGameTest.GetValidGame(tournament);
            game.Teams[0].AddBonus(10);
            game.Teams[0].Players[0].AddMalus(20);
            game.SetTeamRank(game.Teams[0], 1);
            game.SetTeamRank(game.Teams[1], 2);
            return tournament;
        }

        [Fact]
        public void ShouldGetScore()
        {
            Tournament tournament = TournamentTest.GetValidTournament();
            Assert.Equal(0, tournament.GetScore("Joyeux"));
        }

        [Fact]
        public void ShouldThrowIfNameDoesNotExists()
        {
            Tournament tournament = new();
            Assert.Throws<ArgumentException>(() => tournament.GetScore("Pierre"));
        }

        [Fact]
        public void ShouldGetScoreAfterAGame()
        {
            Tournament tournament = GetFinishedTournament();
            Assert.Equal(90, tournament.GetScore("Grincheux"));
        }
    }
}
