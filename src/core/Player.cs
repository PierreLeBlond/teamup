namespace Core
{
    public class Player
    {
        private readonly string name;
        public string Name => name;

        private int bonus;
        public int Bonus => bonus;

        private int malus;
        public int Malus => malus;

        public Player(string name)
        {
            if (name == "")
            {
                throw new ArgumentException("The name cannot be an empty string");
            }
            this.name = name;
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
