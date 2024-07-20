namespace Core.Tests
{
    public class PlayerTest
    {
        public static Player CreatePlayer()
        {
            return new Player("Pierre");
        }

        [Fact]
        public void ShouldCreateAPlayerWithAName()
        {
            Player player = CreatePlayer();
            Assert.Equal("Pierre", player.Name);
        }

        [Fact]
        public void ShouldThrowIfNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new Player(""));
        }

        [Fact]
        public void ShouldCreateAPlayerWithBonusOfZero()
        {
            Player player = CreatePlayer();
            Assert.Equal(0, player.Bonus);
        }

        [Fact]
        public void ShouldAddABonusToPlayer()
        {
            Player player = CreatePlayer();
            player.AddBonus(10);
            player.AddBonus(10);
            Assert.Equal(20, player.Bonus);
        }

        [Fact]
        public void ShouldCreateAPlayerWithMalusOfZero()
        {
            Player player = CreatePlayer();
            Assert.Equal(0, player.Bonus);
        }

        [Fact]
        public void ShouldAddAMalusToPlayer()
        {
            Player player = CreatePlayer();
            player.AddMalus(10);
            player.AddMalus(10);
            Assert.Equal(20, player.Malus);
        }
    }
}
