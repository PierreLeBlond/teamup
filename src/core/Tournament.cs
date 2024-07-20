using System.Globalization;

namespace Core
{
    public class Tournament()
    {
        private readonly HashSet<string> names = [];
        public string[] Names => [.. names];

        private readonly List<Game> games = [];
        public Game[] Games => [.. games];

        public void AddNames(string[] names)
        {
            if (names.Contains(""))
            {
                throw new ArgumentException("A name cannot be an empty string");
            }

            this.names.UnionWith(names);
        }

        private Team[] CreateFairTeams(int numberOfTeam)
        {
            Team[] teams = Array.ConvertAll(new Team[numberOfTeam], _ => new Team());
            int maxTeamSize = names.Count / numberOfTeam + 1;

            // 1. Store scores to avoid recomputation
            Dictionary<string, int> scoreFromNames = [];
            foreach (string name in names)
            {
                scoreFromNames.Add(name, GetScore(name));
            }

            // 2. Keep track of teams scores
            Dictionary<Team, int> scoreFromTeams = [];
            foreach (Team team in teams)
            {
                scoreFromTeams.Add(team, 0);
            }

            Team getLowestScoringTeam()
            {
                return scoreFromTeams.MinBy(team => team.Value).Key;
            }

            foreach (string name in names)
            {
                Team team = getLowestScoringTeam();

                team.AddPlayer(name);
                scoreFromTeams[team] += scoreFromNames[name];

                if (team.Players.Length == maxTeamSize)
                {
                    scoreFromTeams.Remove(team);
                }
            }

            return teams;
        }

        public Game CreateGame(int[] teamRewards)
        {
            if (names.Count < 2)
            {
                throw new InvalidOperationException("The tournament must have at least two names");
            }

            int numberOfTeam = teamRewards.Length;

            if (numberOfTeam < 2)
            {
                throw new ArgumentException("The game must have at least two teams");
            }

            Game game = new();
            games.Add(game);

            Team[] fairTeams = CreateFairTeams(numberOfTeam);

            foreach (Team fairTeam in fairTeams)
            {
                game.AddTeam(fairTeam);
            }

            foreach (int reward in teamRewards)
            {
                game.AddReward(reward);
            }

            return game;
        }

        private static int GetGameScore(Game game, string name)
        {
            int score = 0;

            Team? team = game.GetTeam(name);

            if (team is null)
            {
                return score;
            }

            score += team.Bonus;
            score -= team.Malus;

            Player? player = team.GetPlayer(name);

            if (player is null)
            {
                return score;
            }

            score += player.Bonus;
            score -= player.Malus;

            int? rank = game.GetTeamRank(team);

            if (rank is null)
            {
                return score;
            }

            score += game.GetReward(team) ?? 0;

            return score;
        }

        public int GetScore(string name)
        {
            if (!names.Contains(name))
            {
                throw new ArgumentException("The name does not exist in the tournament");
            }

            int score = 0;

            foreach (Game game in games)
            {
                score += GetGameScore(game, name);
            }

            return score;
        }
    }
}
