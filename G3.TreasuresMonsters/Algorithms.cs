using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters;

public static class Algorithms
{
    public static Random rng = new Random();

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
            int treasureCount = rng.Next(0, 3); // 0, 1 ou 2

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

            int position = rng.Next(0, monstersRow.Length);
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
            int strength = rng.Next(1, 51);
            monstersRow[pos] = strength;
            totalMonsterStrength += strength;

            PlaceMonsters(monstersRow, monsterPositions, ref totalMonsterStrength, index + 1);
        }

        private static void EnsureTreasures(int[] treasuresRow, List<int> treasurePositions, int target, List<int> monsterPositions)
        {
            if (treasurePositions.Count >= target)
                return;

            int position = rng.Next(0, treasuresRow.Length);
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
            int value = rng.Next(1, 100);
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
                double rand = rng.NextDouble();
                if (rand < 1.0 / 6)
                {
                    int value = rng.Next(1, 100);
                    treasuresRow[x] = value;
                    totalTreasureValue += value;
                    treasurePositions.Add(x);
                }
                else if (rand < (1.0 / 6) + (1.0 / 3))
                {
                    int strength = rng.Next(1, 51);
                    monstersRow[x] = strength;
                    totalMonsterStrength += strength;
                    monsterPositions.Add(x);
                }
            }

            ApplyProbabilities(monstersRow, treasuresRow, x + 1, monsterPositions, treasurePositions, ref totalMonsterStrength, ref totalTreasureValue);
        }
    }

    /* --- Divide & Conquer --- */
    public static class DC
    {
        // Signature de la méthode en Java :
        // void sortLevel(int[][] monstersToSort, int[][] treasuresToSort)
        public static void SortLevel(int[][] monstersToSort, int[][] treasuresToSort)
        {
            int height = monstersToSort.Length;

            // Créer une liste des rangées avec leur valeur
            var rows = new List<(int index, int rowValue)>();
            ComputeRowValues(monstersToSort, treasuresToSort, 0, height, rows);

            // Trier les rangées avec un tri fusion récursif
            var sortedRows = MergeSortRows(rows);

            // Reconstruire les tableaux avec les rangées triées
            RebuildArrays(monstersToSort, treasuresToSort, sortedRows, 0);
        }

        // Méthodes utilitaires récursives pour DC
        private static void ComputeRowValues(int[][] monsters, int[][] treasures, int y, int height, List<(int index, int rowValue)> rows)
        {
            if (y >= height)
                return;

            int totalTreasure = 0;
            int totalMonsters = 0;

            ComputeRowValuesHelper(monsters[y], treasures[y], 0, ref totalTreasure, ref totalMonsters);

            int rowValue = totalTreasure - totalMonsters;
            rows.Add((y, rowValue));

            ComputeRowValues(monsters, treasures, y + 1, height, rows);
        }

        private static void ComputeRowValuesHelper(int[] monstersRow, int[] treasuresRow, int x, ref int totalTreasure, ref int totalMonsters)
        {
            if (x >= monstersRow.Length)
                return;

            if (treasuresRow[x] > 0)
                totalTreasure += treasuresRow[x];
            if (monstersRow[x] > 0)
                totalMonsters += monstersRow[x];

            ComputeRowValuesHelper(monstersRow, treasuresRow, x + 1, ref totalTreasure, ref totalMonsters);
        }

        private static List<(int index, int rowValue)> MergeSortRows(List<(int index, int rowValue)> rows)
        {
            if (rows.Count <= 1)
                return rows;

            int mid = rows.Count / 2;
            var left = MergeSortRows(rows.GetRange(0, mid));
            var right = MergeSortRows(rows.GetRange(mid, rows.Count - mid));

            return Merge(left, right);
        }

        private static List<(int index, int rowValue)> Merge(List<(int index, int rowValue)> left, List<(int index, int rowValue)> right)
        {
            var result = new List<(int index, int rowValue)>();
            MergeHelper(left, right, result, 0, 0);
            return result;
        }

        private static void MergeHelper(List<(int index, int rowValue)> left, List<(int index, int rowValue)> right, List<(int index, int rowValue)> result, int i, int j)
        {
            if (i >= left.Count && j >= right.Count)
                return;

            if (i < left.Count && (j >= right.Count || left[i].rowValue <= right[j].rowValue))
            {
                result.Add(left[i]);
                MergeHelper(left, right, result, i + 1, j);
            }
            else
            {
                result.Add(right[j]);
                MergeHelper(left, right, result, i, j + 1);
            }
        }

        private static void RebuildArrays(int[][] monsters, int[][] treasures, List<(int index, int rowValue)> sortedRows, int i)
        {
            if (i >= sortedRows.Count)
                return;

            CopyRow(monsters, sortedRows[i].index, i, 0);
            CopyRow(treasures, sortedRows[i].index, i, 0);

            RebuildArrays(monsters, treasures, sortedRows, i + 1);
        }

        private static void CopyRow(int[][] array, int srcRow, int destRow, int x)
        {
            if (x >= array[srcRow].Length)
                return;

            array[destRow][x] = array[srcRow][x];
            CopyRow(array, srcRow, destRow, x + 1);
        }
    }

    /* --- Greedy Search --- */
    public static class GS
    {
        // Signature de la méthode en Java :
        // int greedySolution(State state)
        public static int GreedySolution(State state)
        {
            int maxScore = GreedySearch(state.HeroX, state.HeroY, state.HeroHealth, 0, state, 5);
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

    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature de la méthode en Java :
        // String perfectSolution(State state)
        public static string PerfectSolution(State state)
        {
            var memo = new Dictionary<(int x, int y, int health), (int score, string path)>();
            var result = DP_Search(state.HeroX, state.HeroY, state.HeroHealth, state, memo);
            return result.path;
        }

        // Méthodes utilitaires récursives pour DP
        public static (int score, string path) DP_Search(int x, int y, int health, State state, Dictionary<(int x, int y, int health), (int score, string path)> memo)
        {
            int height = state.Monsters.Length;

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
            int[] dx = { 0, -1, 1 };
            int[] dy = { 1, 0, 0 };
            char[] moveChar = { 'D', 'L', 'R' };

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

            int width = state.Monsters[0].Length;

            if (nx >= 0 && nx < width && ny >= 0)
            {
                int newHealth = health;
                int treasureCollected = 0;

                if (state.Monsters[ny][nx] > 0)
                {
                    newHealth -= state.Monsters[ny][nx];
                }
                if (state.Treasures[ny][nx] > 0)
                {
                    treasureCollected += state.Treasures[ny][nx];
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