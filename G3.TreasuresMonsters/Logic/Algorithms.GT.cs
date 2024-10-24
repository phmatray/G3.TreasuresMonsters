namespace G3.TreasuresMonsters.Logic
{
    public static partial class Algorithms
    {
        public static int Seed { get; private set; } = 0;
        
        public static Random Rng { get; private set; } = new(Seed);
        
        public static void SetSeed(int seed)
        {
            Seed = seed;
            Rng = new Random(Seed);
        }
        
        /* --- Generate & Test --- */
        public static class GT
        {
            // Public method to generate monsters and treasures for the entire grid
            public static void GenerateMonstersAndTreasures(int[][] monstersToFill, int[][] treasuresToFill)
            {
                int height = monstersToFill.Length;
                HashSet<string> uniqueRows = new HashSet<string>();
                
                for (int y = 0; y < height; y++)
                {
                    int[] monstersRow;
                    int[] treasuresRow;
                    string rowSignature;
                    
                    do
                    {
                        monstersRow = new int[monstersToFill[y].Length];
                        treasuresRow = new int[treasuresToFill[y].Length];
                        GenerateRow(monstersRow, treasuresRow, Rng);
                        rowSignature = GetRowSignature(monstersRow, treasuresRow);
                    } while (!uniqueRows.Add(rowSignature) || !IsValidRow(monstersRow, treasuresRow)); // Repeat if row is not unique or not valid
                    
                    Array.Copy(monstersRow, monstersToFill[y], monstersRow.Length);
                    Array.Copy(treasuresRow, treasuresToFill[y], treasuresRow.Length);
                }
            }

            // Method to generate a single row of monsters and treasures
            private static void GenerateRow(int[] monstersRow, int[] treasuresRow, Random rng)
            {
                InitializeRow(monstersRow, treasuresRow);
                ApplyProbabilities(monstersRow, treasuresRow, rng);
            }

            // Method to initialize a row by setting all cells to zero
            private static void InitializeRow(int[] monstersRow, int[] treasuresRow)
            {
                Array.Clear(monstersRow, 0, monstersRow.Length); // Clear the monsters row
                Array.Clear(treasuresRow, 0, treasuresRow.Length); // Clear the treasures row
            }

            // Method to apply probabilities to the cells in the row
            private static void ApplyProbabilities(int[] monstersRow, int[] treasuresRow, Random rng)
            {
                int monsterCount = 0;
                int treasureCount = 0;
                List<int> availablePositions = Enumerable.Range(0, monstersRow.Length).ToList();
                
                // Shuffle positions to ensure randomness
                availablePositions = availablePositions.OrderBy(x => rng.Next()).ToList();

                foreach (int x in availablePositions)
                {
                    double rand = rng.NextDouble(); // Random value between 0 and 1
                    if (rand < 1.0 / 6 && treasureCount < 2) // 1 in 6 chance to place a treasure, max 2 treasures
                    {
                        treasuresRow[x] = rng.Next(1, 100); // Random treasure value between 1 and 99
                        treasureCount++;
                    }
                    else if (rand < (1.0 / 6) + (1.0 / 3) && monsterCount < 2) // 1 in 3 chance to place a monster, max 2 monsters
                    {
                        monstersRow[x] = rng.Next(1, 51); // Random monster strength between 1 and 50
                        monsterCount++;
                    }
                    // Stop if we have placed enough monsters and treasures
                    if (monsterCount == 2 && treasureCount == 2)
                    {
                        break;
                    }
                }
            }

            // Method to validate if the generated row meets the required conditions
            private static bool IsValidRow(int[] monstersRow, int[] treasuresRow)
            {
                int monsterCount = monstersRow.Count(x => x > 0); // Count the number of monsters
                int treasureCount = treasuresRow.Count(x => x > 0); // Count the number of treasures
                int totalMonsterStrength = monstersRow.Sum(); // Sum of all monster strengths
                int totalTreasureValue = treasuresRow.Sum(); // Sum of all treasure values

                // A valid row must have exactly 2 monsters, at most 2 treasures, and the total treasure value must not exceed total monster strength
                return monsterCount == 2 && treasureCount <= 2 && totalTreasureValue <= totalMonsterStrength;
            }

            // Method to generate a unique signature for a row
            private static string GetRowSignature(int[] monstersRow, int[] treasuresRow)
            {
                return string.Join(",", monstersRow) + "|" + string.Join(",", treasuresRow);
            }
        }
    }
}
