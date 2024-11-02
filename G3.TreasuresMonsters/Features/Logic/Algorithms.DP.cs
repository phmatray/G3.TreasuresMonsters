namespace G3.TreasuresMonsters.Features.Logic;

public record MemoKey(int X, int Y, int Health);

public record MemoValue(int Health, int TreasureCollected, string Path)
{
    public int TotalScore => Health + TreasureCollected;
}

public static partial class Algorithms
{
    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature de la méthode en Java :
        // String perfectSolution(State state)
        // Method to find the perfect solution using bottom-up DP
        public static string PerfectSolution(State state)
        {
            // Initialize DP table
            var heroHealth = state.HeroHealth;
            var height = state.Monsters.Length;
            var width = state.Monsters[0].Length;
            var cache = new MemoValue?[height + 1, width, heroHealth + 1];
            var moves = Constants.GetMoves(); // Should return ["↓", "←", "→"]

            // Base case: At the end of the dungeon, starting from any x with health > 0
            for (int x = 0; x < width; x++)
            {
                for (int h = 1; h <= heroHealth; h++)
                {
                    cache[height, x, h] = new MemoValue(h, 0, "");
                }
            }

            // Build the DP table from bottom to top
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int health = 1; health <= heroHealth; health++)
                    {
                        MemoValue? bestResult = null;

                        // Possible moves: Down, Left, Right
                        int[] dx = [0, -1, 1];
                        int[] dy = [1, 0, 0];
                        string[] moveSymbols = moves;

                        for (int i = 0; i < dx.Length; i++)
                        {
                            int nx = x + dx[i];
                            int ny = y + dy[i];

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                int newHealth = health;
                                int treasureCollected = 0;

                                // Apply the effects of moving to the next cell
                                ApplyCellEffects(nx, ny, state, ref newHealth, ref treasureCollected);

                                // If health drops to zero or below, skip this path
                                if (newHealth <= 0)
                                    continue;

                                var nextResult = cache[ny, nx, newHealth];

                                if (nextResult != null)
                                {
                                    int totalTreasure = nextResult.TreasureCollected + treasureCollected;
                                    int totalScore = newHealth + totalTreasure;

                                    if (bestResult == null || totalScore > bestResult.TotalScore)
                                    {
                                        bestResult = new MemoValue(newHealth, totalTreasure,
                                            moveSymbols[i] + nextResult.Path);
                                    }
                                }
                            }
                            else if (ny == height) // Reached the end of the dungeon
                            {
                                // No cell effects at the end
                                if (bestResult == null || health > bestResult.TotalScore)
                                {
                                    bestResult = new MemoValue(health, 0, moveSymbols[i]);
                                }
                            }
                        }

                        if (bestResult != null)
                        {
                            cache[y, x, health] = bestResult;
                        }
                    }
                }
            }

            // Apply cell effects at the starting cell
            int startX = state.HeroX;
            int startY = state.HeroY;
            int startHealth = state.HeroHealth;
            int initialHealth = startHealth;
            int initialTreasure = 0;
            ApplyCellEffects(startX, startY, state, ref initialHealth, ref initialTreasure);

            if (initialHealth <= 0)
            {
                return "";
            }

            MemoValue? result = cache[startY, startX, initialHealth];
            return result?.Path ?? "";
        }

        // Apply the effects of the cell (monsters and treasures)
        private static void ApplyCellEffects(int x, int y, State state, ref int health, ref int treasureCollected)
        {
            // Subtract monster strength from health
            health -= state.Monsters[y][x];

            // Add treasure value to treasureCollected
            treasureCollected += state.Treasures[y][x];
        }
    }
}