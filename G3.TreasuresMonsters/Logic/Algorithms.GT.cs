namespace G3.TreasuresMonsters.Logic;

public static partial class Algorithms
{
    /* --- Generate & Test --- */
    public static class GT
    {
        // Signature de la méthode en Java :
        // void generateMonstersAndTreasures(int[][] monstersToFill, int[][] treasuresToFill)
        public static void GenerateMonstersAndTreasures(int[][] monstersToFill, int[][] treasuresToFill)
        {
            int height = monstersToFill.Length;
            GenerateRows(monstersToFill, treasuresToFill, 0, height);
        }

        // Méthodes utilitaires récursives
        private static void GenerateRows(int[][] monsters, int[][] treasures, int y, int height)
        {
            if (y >= height)
                return;

            InitializeRow(monsters[y], treasures[y], 0);

            GenerateRowUntilValid(monsters[y], treasures[y]);

            GenerateRows(monsters, treasures, y + 1, height);
        }

        private static void InitializeRow(int[] monstersRow, int[] treasuresRow, int x)
        {
            if (x >= monstersRow.Length)
                return;

            monstersRow[x] = 0;
            treasuresRow[x] = 0;

            InitializeRow(monstersRow, treasuresRow, x + 1);
        }

        private static void GenerateRowUntilValid(int[] monstersRow, int[] treasuresRow)
        {
            if (TryGenerateRow(monstersRow, treasuresRow))
                return;
            else
            {
                InitializeRow(monstersRow, treasuresRow, 0);
                GenerateRowUntilValid(monstersRow, treasuresRow);
            }
        }

        private static bool TryGenerateRow(int[] monstersRow, int[] treasuresRow)
        {
            List<int> monsterPositions = new List<int>();
            List<int> treasurePositions = new List<int>();
            int totalMonsterStrength = 0;
            int totalTreasureValue = 0;

            // Assurer au moins 2 monstres
            EnsureMonsters(monstersRow, monsterPositions, 2);

            // Placer les monstres
            PlaceMonsters(monstersRow, monsterPositions, ref totalMonsterStrength, 0);

            // Décider du nombre de trésors (max 2)
            int treasureCount = Rng.Next(0, 3); // 0, 1 ou 2

            // Assurer les trésors
            EnsureTreasures(treasuresRow, treasurePositions, treasureCount, monsterPositions);

            // Placer les trésors
            PlaceTreasures(treasuresRow, treasurePositions, ref totalTreasureValue, 0);

            // Appliquer les probabilités aux autres cellules
            ApplyProbabilities(monstersRow, treasuresRow, 0, monsterPositions, treasurePositions, ref totalMonsterStrength, ref totalTreasureValue);

            // Valider les contraintes
            if (monsterPositions.Count >= 2 && treasurePositions.Count <= 2 &&
                totalTreasureValue <= totalMonsterStrength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Méthodes utilitaires récursives pour GT
        private static void EnsureMonsters(int[] monstersRow, List<int> monsterPositions, int target)
        {
            if (monsterPositions.Count >= target)
                return;

            int position = Rng.Next(0, monstersRow.Length);
            if (!monsterPositions.Contains(position))
            {
                monsterPositions.Add(position);
            }

            EnsureMonsters(monstersRow, monsterPositions, target);
        }

        private static void PlaceMonsters(int[] monstersRow, List<int> monsterPositions, ref int totalMonsterStrength, int index)
        {
            if (index >= monsterPositions.Count)
                return;

            int pos = monsterPositions[index];
            int strength = Rng.Next(1, 51);
            monstersRow[pos] = strength;
            totalMonsterStrength += strength;

            PlaceMonsters(monstersRow, monsterPositions, ref totalMonsterStrength, index + 1);
        }

        private static void EnsureTreasures(int[] treasuresRow, List<int> treasurePositions, int target, List<int> monsterPositions)
        {
            if (treasurePositions.Count >= target)
                return;

            int position = Rng.Next(0, treasuresRow.Length);
            if (!monsterPositions.Contains(position) && !treasurePositions.Contains(position))
            {
                treasurePositions.Add(position);
            }

            EnsureTreasures(treasuresRow, treasurePositions, target, monsterPositions);
        }

        private static void PlaceTreasures(int[] treasuresRow, List<int> treasurePositions, ref int totalTreasureValue, int index)
        {
            if (index >= treasurePositions.Count)
                return;

            int pos = treasurePositions[index];
            int value = Rng.Next(1, 100);
            treasuresRow[pos] = value;
            totalTreasureValue += value;

            PlaceTreasures(treasuresRow, treasurePositions, ref totalTreasureValue, index + 1);
        }

        private static void ApplyProbabilities(int[] monstersRow, int[] treasuresRow, int x, List<int> monsterPositions, List<int> treasurePositions, ref int totalMonsterStrength, ref int totalTreasureValue)
        {
            if (x >= monstersRow.Length)
                return;

            if (monstersRow[x] == 0 && treasuresRow[x] == 0)
            {
                double rand = Rng.NextDouble();
                if (rand < 1.0 / 6)
                {
                    int value = Rng.Next(1, 100);
                    treasuresRow[x] = value;
                    totalTreasureValue += value;
                    treasurePositions.Add(x);
                }
                else if (rand < (1.0 / 6) + (1.0 / 3))
                {
                    int strength = Rng.Next(1, 51);
                    monstersRow[x] = strength;
                    totalMonsterStrength += strength;
                    monsterPositions.Add(x);
                }
            }

            ApplyProbabilities(monstersRow, treasuresRow, x + 1, monsterPositions, treasurePositions, ref totalMonsterStrength, ref totalTreasureValue);
        }
    }
}