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
        public static string PerfectSolution(State state)
        {
            var memo = new Dictionary<MemoKey, MemoValue>();
            var result = DP_Search(state.HeroX, state.HeroY, state.HeroHealth, state, memo);
            return result?.Path ?? "";
        }

        // Recursive method to search for the best path
        private static MemoValue DP_Search(int x, int y, int health, State state, Dictionary<MemoKey, MemoValue> memo)
        {
            int height = state.Monsters.Length;
            int width = state.Monsters[0].Length;

            // Base cases
            if (health <= 0)
            {
                // Hero is dead
                return null;
            }

            if (y >= height)
            {
                // Reached the end of the dungeon successfully
                return new MemoValue(health, 0, "");
            }

            var key = new MemoKey(x, y, health);

            if (memo.TryGetValue(key, out MemoValue memoizedResult))
            {
                // Return the memoized result
                return memoizedResult;
            }

            MemoValue bestResult = null;

            // Possible moves: Down, Left, Right
            int[] dx = { 0, -1, 1 };
            int[] dy = { 1, 0, 0 };
            char[] moveChar = { '↓', '←', '→' };

            for (int i = 0; i < dx.Length; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (IsInBounds(nx, ny, width, height))
                {
                    int newHealth = health;
                    int treasureCollected = 0;

                    // Apply the effects of the cell
                    ApplyCellEffects(nx, ny, state, ref newHealth, ref treasureCollected);

                    var result = DP_Search(nx, ny, newHealth, state, memo);

                    if (result != null)
                    {
                        int totalHealth = result.Health;
                        int totalTreasure = result.TreasureCollected + treasureCollected;
                        int totalScore = totalHealth + totalTreasure;

                        if (bestResult == null || totalScore > bestResult.TotalScore)
                        {
                            bestResult = new MemoValue(totalHealth, totalTreasure, moveChar[i] + result.Path);
                        }
                    }
                }
            }

            // Update memoization dictionary
            memo[key] = bestResult ?? new MemoValue(0, 0, "");

            return memo[key];
        }

        // Check if the next position is within bounds
        private static bool IsInBounds(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0;
        }

        // Apply the effects of the cell (monsters and treasures)
        private static void ApplyCellEffects(int x, int y, State state, ref int health, ref int treasureCollected)
        {
            int height = state.Monsters.Length;

            if (y < height)
            {
                // Subtract monster strength from health
                health -= state.Monsters[y][x];

                // Add treasure value to treasureCollected
                treasureCollected += state.Treasures[y][x];
            }
        }
    }
}