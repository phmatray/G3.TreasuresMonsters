namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
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
}