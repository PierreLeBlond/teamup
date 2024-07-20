namespace Core
{
    public class Game()
    {
        private Team[] teams = [];
        public Team[] Teams => teams;

        private readonly LinkedList<int> rewards = new();

        private readonly List<Team> rankedTeams = [];

        public void AddTeam(Team team)
        {
            int length = teams.Length;
            Array.Resize(ref teams, length + 1);
            teams[length] = team;
        }

        public Team? GetTeam(string name)
        {
            return Array.Find(teams, team => team.GetPlayer(name) is not null);
        }

        public void AddReward(int reward)
        {
            rewards.AddLast(reward);
        }

        public int? GetReward(Team team)
        {
            int rank = rankedTeams.FindIndex(rankedTeam => rankedTeam == team);

            if (rank < 0)
            {
                return null;
            }

            return rewards.ElementAt(rank);
        }

        public void SetTeamRank(Team team, int rank)
        {
            if (!teams.Contains(team))
            {
                throw new ArgumentException("The team does not exist in the game");
            }

            rankedTeams.Remove(team);

            if (rank < 1 || rank > rankedTeams.Count + 1)
            {
                throw new ArgumentException(
                    "The rank must be between 1 and " + (rankedTeams.Count + 1)
                );
            }

            rankedTeams.Insert(rank - 1, team);
        }

        public int? GetTeamRank(Team team)
        {
            if (!teams.Contains(team))
            {
                throw new ArgumentException("The team does not exist in the game");
            }

            if (!rankedTeams.Contains(team))
            {
                return null;
            }

            return rankedTeams.IndexOf(team) + 1;
        }
    }
}
