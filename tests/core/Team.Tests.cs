namespace Core.Tests
{
    public class TeamTest
    {
        public static Team CreateTeam()
        {
            return new Team();
        }

        [Fact]
        public void ShouldCreateATeam()
        {
            Team team = CreateTeam();
            Assert.NotNull(team);
        }

        [Fact]
        public void ShouldAddAPlayer()
        {
            Team team = CreateTeam();
            team.AddPlayer("Pierre");
            Assert.Equal("Pierre", team.Players[0].Name);
        }

        [Fact]
        public void ShouldGetPlayer()
        {
            Team team = CreateTeam();
            team.AddPlayer("Pierre");
            Assert.Equal("Pierre", team.GetPlayer("Pierre")?.Name);
        }

        [Fact]
        public void ShouldGetNullIfPlayerDoesNotExists()
        {
            Team team = CreateTeam();
            Assert.Null(team.GetPlayer("Pierre"));
        }

        [Fact]
        public void ShouldCreateATeamWithBonusOfZero()
        {
            Team team = CreateTeam();
            Assert.Equal(0, team.Bonus);
        }

        [Fact]
        public void ShouldAddABonusToTeam()
        {
            Team team = CreateTeam();
            team.AddBonus(10);
            team.AddBonus(10);
            Assert.Equal(20, team.Bonus);
        }

        [Fact]
        public void ShouldCreateATeamWithMalusOfZero()
        {
            Team team = CreateTeam();
            Assert.Equal(0, team.Malus);
        }

        [Fact]
        public void ShouldAddAMalusToTeam()
        {
            Team team = CreateTeam();
            team.AddMalus(10);
            team.AddMalus(10);
            Assert.Equal(20, team.Malus);
        }
    }
}
