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
            int[] rowValues = new int[height];
            int[] indices = new int[height];

            // Compute row values and initialize indices
            for (int i = 0; i < height; i++)
            {
                int monsterSum = SumArray(monstersToSort[i]);
                int treasureSum = SumArray(treasuresToSort[i]);
                rowValues[i] = treasureSum - monsterSum;
                indices[i] = i;
            }

            // Perform recursive merge sort on indices based on rowValues
            MergeSort(indices, rowValues, 0, indices.Length - 1);

            // Rebuild the arrays with sorted rows
            RebuildArrays(monstersToSort, treasuresToSort, indices);
        }

        // Method to sum array elements
        internal static int SumArray(int[] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        // Recursive merge sort implementation on indices
        internal static void MergeSort(int[] indices, int[] rowValues, int left, int right)
        {
            if (left >= right)
                return;

            int mid = left + (right - left) / 2;
            MergeSort(indices, rowValues, left, mid);
            MergeSort(indices, rowValues, mid + 1, right);
            Merge(indices, rowValues, left, mid, right);
        }

        // Merge two sorted subarrays
        internal static void Merge(int[] indices, int[] rowValues, int left, int mid, int right)
        {
            int leftSize = mid - left + 1;
            int rightSize = right - mid;

            int[] leftIndices = new int[leftSize];
            int[] rightIndices = new int[rightSize];

            // Copy data to temporary arrays
            for (int i = 0; i < leftSize; i++)
                leftIndices[i] = indices[left + i];
            for (int j = 0; j < rightSize; j++)
                rightIndices[j] = indices[mid + 1 + j];

            int iLeft = 0, iRight = 0, k = left;

            // Merge the temp arrays back into indices
            while (iLeft < leftSize && iRight < rightSize)
            {
                if (rowValues[leftIndices[iLeft]] <= rowValues[rightIndices[iRight]])
                {
                    indices[k++] = leftIndices[iLeft++];
                }
                else
                {
                    indices[k++] = rightIndices[iRight++];
                }
            }

            // Copy any remaining elements of leftIndices
            while (iLeft < leftSize)
            {
                indices[k++] = leftIndices[iLeft++];
            }

            // Copy any remaining elements of rightIndices
            while (iRight < rightSize)
            {
                indices[k++] = rightIndices[iRight++];
            }
        }

        // Rebuild the original arrays based on sorted indices
        internal static void RebuildArrays(int[][] monsters, int[][] treasures, int[] indices)
        {
            int[][] sortedMonsters = new int[monsters.Length][];
            int[][] sortedTreasures = new int[treasures.Length][];

            for (int i = 0; i < indices.Length; i++)
            {
                sortedMonsters[i] = monsters[indices[i]];
                sortedTreasures[i] = treasures[indices[i]];
            }

            // Copy sorted arrays back to original arrays
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i] = sortedMonsters[i];
                treasures[i] = sortedTreasures[i];
            }
        }
    }
}