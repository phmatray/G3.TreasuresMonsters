using G3.TreasuresMonsters.Features.Logic;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsDCTests
{
    [SetUp]
    public void SetUp()
    {
        // If there is any setup needed before each test, it can be added here.
        // For example, if Algorithms.DC uses a random number generator that needs seeding.
        // Since the DC algorithm is deterministic, no setup is required.
    }

    [Test]
    public void SortLevel_Should_Sort_Rows_By_RowValue()
    {
        // Arrange
        int[][] monstersToSort =
        [
            [5, 0, 10],   // Monster sum = 15
            [0, 0, 0],    // Monster sum = 0
            [3, 7, 0],    // Monster sum = 10
            [0, 2, 0] // Monster sum = 2
        ];

        int[][] treasuresToSort =
        [
            [0, 20, 0],   // Treasure sum = 20
            [10, 0, 5],   // Treasure sum = 15
            [0, 0, 0],    // Treasure sum = 0
            [0, 0, 15] // Treasure sum = 15
        ];

        // Expected row values (Treasure sum - Monster sum)
        // Row 0: 20 - 15 = 5
        // Row 1: 15 - 0 = 15
        // Row 2: 0 - 10 = -10
        // Row 3: 15 - 2 = 13

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Compute actual row values after sorting
        int[] actualRowValues = new int[monstersToSort.Length];
        for (int i = 0; i < monstersToSort.Length; i++)
        {
            int monsterSum = monstersToSort[i].Sum();
            int treasureSum = treasuresToSort[i].Sum();
            actualRowValues[i] = treasureSum - monsterSum;
        }

        // Expected sorted row values in ascending order: -10, 5, 13, 15
        int[] expectedRowValues = [-10, 5, 13, 15];

        // Assert
        Assert.That(actualRowValues, Is.EqualTo(expectedRowValues), "Rows are not sorted correctly by RowValue.");
    }

    [Test]
    public void SortLevel_Should_Handle_Empty_Rows_Correctly()
    {
        // Arrange
        int[][] monstersToSort =
        [
            [0],// Monster sum = 0
            [1],// Monster sum = 1
            [0] // Monster sum = 0
        ];

        int[][] treasuresToSort =
        [
            [0],// Treasure sum = 0
            [2],// Treasure sum = 2
            [0] // Treasure sum = 0
        ];

        // Expected row values:
        // Row 0: 0 - 0 = 0
        // Row 1: 2 - 1 = 1
        // Row 2: 0 - 0 = 0

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Compute actual row values after sorting
        int[] actualRowValues = new int[monstersToSort.Length];
        for (int i = 0; i < monstersToSort.Length; i++)
        {
            int monsterSum = monstersToSort[i].Sum();
            int treasureSum = treasuresToSort[i].Sum();
            actualRowValues[i] = treasureSum - monsterSum;
        }

        // Expected sorted row values in ascending order: 0, 0, 1
        int[] expectedRowValues = [0, 0, 1];

        // Assert
        Assert.That(actualRowValues, Is.EqualTo(expectedRowValues), "Empty rows are not handled correctly.");
    }

    [Test]
    public void SortLevel_Should_Handle_Single_Row()
    {
        // Arrange
        int[][] monstersToSort =
        [
            [10, 20, 30]
        ];

        int[][] treasuresToSort =
        [
            [5, 15, 25]
        ];

        // Expected row value: (5 + 15 + 25) - (10 + 20 + 30) = 45 - 60 = -15

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Compute actual row value after sorting
        int monsterSum = monstersToSort[0].Sum();
        int treasureSum = treasuresToSort[0].Sum();
        int actualRowValue = treasureSum - monsterSum;

        // Expected row value
        int expectedRowValue = -15;

        // Assert
        Assert.That(actualRowValue, Is.EqualTo(expectedRowValue), "Single row is not handled correctly.");
    }

    [Test]
    public void SortLevel_Should_Sort_Rows_With_Same_RowValue_Correctly()
    {
        // Arrange
        int[][] monstersToSort =
        [
            [5, 0, 0],// Monster sum = 5
            [3, 2, 0],// Monster sum = 5
            [1, 1, 3] // Monster sum = 5
        ];

        int[][] treasuresToSort =
        [
            [10, 0, 0],// Treasure sum = 10
            [ 7, 3, 0],// Treasure sum = 10
            [ 2, 2, 6] // Treasure sum = 10
        ];

        // All rows have RowValue = 10 - 5 = 5

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Compute actual row values after sorting
        int[] actualRowValues = new int[monstersToSort.Length];
        for (int i = 0; i < monstersToSort.Length; i++)
        {
            int monsterSum = monstersToSort[i].Sum();
            int treasureSum = treasuresToSort[i].Sum();
            actualRowValues[i] = treasureSum - monsterSum;
        }

        // All expected row values are 5
        int[] expectedRowValues = [5, 5, 5];

        // Assert
        Assert.That(actualRowValues, Is.EqualTo(expectedRowValues), "Rows with same RowValue are not sorted correctly.");
    }

    [Test]
    public void SortLevel_Should_Not_Modify_Row_Content()
    {
        // Arrange
        int[][] monstersToSort =
        [
            [4, 0],
            [1, 1],
            [0, 4]
        ];

        int[][] treasuresToSort =
        [
            [0, 8],
            [5, 3],
            [8, 0]
        ];

        // Copy the original row contents for comparison after sorting
        int[][] expectedMonsters = monstersToSort.Select(row => (int[])row.Clone()).ToArray();
        int[][] expectedTreasures = treasuresToSort.Select(row => (int[])row.Clone()).ToArray();

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Assert that the content of the rows has not been modified (order may have changed)
        // We need to compare the sets of rows, regardless of their order
        Assert.That(monstersToSort, Is.EquivalentTo(expectedMonsters), "Monsters rows have been modified.");
        Assert.That(treasuresToSort, Is.EquivalentTo(expectedTreasures), "Treasures rows have been modified.");
    }

    [Test]
    public void SortLevel_Should_Handle_Large_Input()
    {
        // Arrange
        const int height = 100;
        const int width = 50;
        int[][] monstersToSort = new int[height][];
        int[][] treasuresToSort = new int[height][];
        Random rnd = new Random(42);

        for (int i = 0; i < height; i++)
        {
            monstersToSort[i] = new int[width];
            treasuresToSort[i] = new int[width];

            for (int j = 0; j < width; j++)
            {
                monstersToSort[i][j] = rnd.Next(0, 10);
                treasuresToSort[i][j] = rnd.Next(0, 10);
            }
        }

        // Act
        Algorithms.DC.SortLevel(monstersToSort, treasuresToSort);

        // Compute actual row values after sorting
        int[] actualRowValues = new int[height];
        for (int i = 0; i < height; i++)
        {
            int monsterSum = monstersToSort[i].Sum();
            int treasureSum = treasuresToSort[i].Sum();
            actualRowValues[i] = treasureSum - monsterSum;
        }

        // Assert that the row values are in non-decreasing order
        for (int i = 1; i < height; i++)
        {
            Assert.That(actualRowValues[i], Is.GreaterThanOrEqualTo(actualRowValues[i - 1]), $"Row {i} is not sorted correctly.");
        }
    }

    [Test]
    public void SumArray_Should_Return_Correct_Sum()
    {
        // Arrange
        int[] array = [1, 2, 3, 4, 5];

        // Act
        int sum = Algorithms.DC.SumArray(array);

        // Assert
        Assert.That(sum, Is.EqualTo(15), "SumArray did not return the correct sum.");
    }
}