using G3.TreasuresMonsters.Features.Engine;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Greedy Search --- */
    public static class GS
    {
        // Signature de la méthode en Java :
        // int greedySolution(State state)
        public static int GreedySolution(State state)
        {
            int maxScore = GreedySearch(state.Hero.X, state.Hero.Y, state.Hero.Health, 0, state, 5);
            return maxScore;
        }

        // Méthodes utilitaires récursives pour GS
        private static int GreedySearch(int x, int y, int health, int treasureCollected, State state, int depth)
        {
            if (depth == 0 || y >= state.Monsters.Length || health <= 0)
            {
                return health + treasureCollected;
            }

            int maxScore = int.MinValue;

            // Déplacements possibles : Bas, Gauche, Droite
            int[] dx = { 0, -1, 1 };
            int[] dy = { 1, 0, 0 };

            maxScore = GreedySearchMoves(x, y, health, treasureCollected, state, depth, dx, dy, 0, maxScore);

            return maxScore;
        }

        private static int GreedySearchMoves(int x, int y, int health, int treasureCollected, State state, int depth, int[] dx, int[] dy, int i, int currentMax)
        {
            if (i >= dx.Length)
                return currentMax;

            int nx = x + dx[i];
            int ny = y + dy[i];

            int width = state.Monsters[0].Length;
            int height = state.Monsters.Length;

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                int newHealth = health;
                int newTreasureCollected = treasureCollected;

                if (state.Monsters[ny][nx] > 0)
                {
                    newHealth -= state.Monsters[ny][nx];
                }
                if (state.Treasures[ny][nx] > 0)
                {
                    newTreasureCollected += state.Treasures[ny][nx];
                }

                if (newHealth > 0)
                {
                    int score = GreedySearch(nx, ny, newHealth, newTreasureCollected, state, depth - 1);
                    if (score > currentMax)
                    {
                        currentMax = score;
                    }
                }
            }

            return GreedySearchMoves(x, y, health, treasureCollected, state, depth, dx, dy, i + 1, currentMax);
        }
    }
}