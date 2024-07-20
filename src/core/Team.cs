namespace Core
{
    public class Team()
    {
        private Player[] players = [];
        public Player[] Players => players;

        private int bonus;
        public int Bonus => bonus;

        private int malus;
        public int Malus => malus;

        public void AddPlayer(string name)
        {
            int length = players.Length;
            Array.Resize(ref players, length + 1);
            players[length] = new Player(name);
        }

        public Player? GetPlayer(string name)
        {
            return Array.Find(players, player => player.Name == name);
        }

        public void AddBonus(int bonus)
        {
            this.bonus += bonus;
        }

        public void AddMalus(int malus)
        {
            this.malus += malus;
        }
    }
}
