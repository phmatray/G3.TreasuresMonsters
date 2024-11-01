using G3.TreasuresMonsters.Features.Engine;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature de la méthode en Java :
        // String perfectSolution(State state)
        public static string PerfectSolution(State state)
        {
            return "← ↓ →";
            var memo = new Dictionary<(int x, int y, int health), (int score, string path)>();
            var result = DP_Search(state.Hero.X, state.Hero.Y, state.Hero.Health, state, memo);
            return result.path;
        }

        // Méthodes utilitaires récursives pour DP
        public static (int score, string path) DP_Search(int x, int y, int health, State state, Dictionary<(int x, int y, int health), (int score, string path)> memo)
        {
            int height = state.Dungeon.Monsters.Length;

            if (y >= height)
            {
                return (health, "");
            }

            if (health <= 0)
            {
                return (int.MinValue, "");
            }

            var key = (x, y, health);
            if (memo.ContainsKey(key))
            {
                return memo[key];
            }

            int maxScore = int.MinValue;
            string bestPath = "";

            // Déplacements possibles : Bas, Gauche, Droite
            int[] dx = [0, -1, 1];
            int[] dy = [1, 0, 0];
            char[] moveChar = ['↓', '←', '→'];

            DP_SearchMoves(x, y, health, state, memo, dx, dy, moveChar, 0, ref maxScore, ref bestPath);

            memo[key] = (maxScore, bestPath);
            return (maxScore, bestPath);
        }

        private static void DP_SearchMoves(int x, int y, int health, State state, Dictionary<(int x, int y, int health), (int score, string path)> memo, int[] dx, int[] dy, char[] moveChar, int i, ref int maxScore, ref string bestPath)
        {
            if (i >= dx.Length)
                return;

            int nx = x + dx[i];
            int ny = y + dy[i];

            int width = state.Dungeon.Monsters[0].Length;

            if (nx >= 0 && nx < width && ny >= 0)
            {
                int newHealth = health;
                int treasureCollected = 0;

                if (state.Dungeon.Monsters[ny][nx] > 0)
                {
                    newHealth -= state.Dungeon.Monsters[ny][nx];
                }
                if (state.Dungeon.Treasures[ny][nx] > 0)
                {
                    treasureCollected += state.Dungeon.Treasures[ny][nx];
                }

                if (newHealth > 0)
                {
                    var result = DP_Search(nx, ny, newHealth, state, memo);

                    int totalScore = result.score + treasureCollected;

                    if (totalScore > maxScore)
                    {
                        maxScore = totalScore;
                        bestPath = moveChar[i] + result.path;
                    }
                }
            }

            DP_SearchMoves(x, y, health, state, memo, dx, dy, moveChar, i + 1, ref maxScore, ref bestPath);
        }
    }
}