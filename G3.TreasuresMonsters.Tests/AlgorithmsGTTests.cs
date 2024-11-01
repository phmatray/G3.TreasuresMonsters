using G3.TreasuresMonsters.Features.Logic;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsGTTests
{
    [SetUp]
    public void SetUp()
    {
        Algorithms.SetSeed(42);
    }

    [Test]
    public void GenerateMonstersAndTreasures_Should_Fill_Grids_With_Valid_Data()
    {
        // Arrange
        const int height = 5;
        const int width = 5;
        var monstersGrid = new int[height][];
        var treasuresGrid = new int[height][];

        for (var i = 0; i < height; i++)
        {
            monstersGrid[i] = new int[width];
            treasuresGrid[i] = new int[width];
        }

        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(monstersGrid, treasuresGrid);

        // Assert
        HashSet<string> uniqueRows = [];
        for (var y = 0; y < height; y++)
        {
            var monstersRow = monstersGrid[y];
            var treasuresRow = treasuresGrid[y];

            // Check if the row is valid
            Assert.That(Algorithms.GT.IsValidRow(monstersRow, treasuresRow), Is.True, $"Row {y} is not valid.");

            // Check for unique row signatures
            var signature = Algorithms.GT.GetRowSignature(monstersRow, treasuresRow);
            Assert.That(uniqueRows.Add(signature), Is.True, $"Row {y} is not unique.");
        }
    }

    [Test]
    public void GenerateRow_Should_Create_Row_With_Valid_Monsters_And_Treasures()
    {
        // Arrange
        const int width = 5;
        var monstersRow = new int[width];
        var treasuresRow = new int[width];

        // Act
        Algorithms.GT.GenerateRow(monstersRow, treasuresRow);

        // Assert
        Assert.That(Algorithms.GT.IsValidRow(monstersRow, treasuresRow), Is.True, "Generated row is not valid.");
    }

    [Test]
    public void IsValidRow_Should_Return_True_For_Valid_Row()
    {
        // Arrange
        int[] monstersRow = [10, 20, 0, 0, 0];
        int[] treasuresRow = [0, 0, 15, 0, 0];

        // Act
        var isValid = Algorithms.GT.IsValidRow(monstersRow, treasuresRow);

        // Assert
        Assert.That(isValid, Is.True, "The row should be valid.");
    }

    [Test]
    public void IsValidRow_Should_Return_False_For_Row_With_Insufficient_Monsters()
    {
        // Arrange
        int[] monstersRow = [10, 0, 0, 0, 0];
        int[] treasuresRow = [0, 0, 0, 0, 0];

        // Act
        var isValid = Algorithms.GT.IsValidRow(monstersRow, treasuresRow);

        // Assert
        Assert.That(isValid, Is.False, "The row should be invalid due to insufficient monsters.");
    }

    [Test]
    public void IsValidRow_Should_Return_False_For_Row_With_Excess_Treasures()
    {
        // Arrange
        int[] monstersRow = [10, 20, 0, 0, 0];
        int[] treasuresRow = [5, 15, 25, 0, 0]; // 3 treasures

        // Act
        var isValid = Algorithms.GT.IsValidRow(monstersRow, treasuresRow);

        // Assert
        Assert.That(isValid, Is.False, "The row should be invalid due to excess treasures.");
    }

    [Test]
    public void IsValidRow_Should_Return_False_When_Treasure_Value_Exceeds_Monster_Strength()
    {
        // Arrange
        int[] monstersRow = [10, 10, 0, 0, 0]; // Total strength = 20
        int[] treasuresRow = [0, 0, 15, 10, 0]; // Total value = 25

        // Act
        var isValid = Algorithms.GT.IsValidRow(monstersRow, treasuresRow);

        // Assert
        Assert.That(isValid, Is.False, "The row should be invalid because treasure value exceeds monster strength.");
    }

    [Test]
    public void GetRowSignature_Should_Return_Correct_Signature()
    {
        // Arrange
        int[] monstersRow = [10, 0, 20, 0, 0];
        int[] treasuresRow = [0, 15, 0, 0, 0];

        // Act
        var signature = Algorithms.GT.GetRowSignature(monstersRow, treasuresRow);

        // Assert
        const string expectedSignature = "10,0,20,0,0,|0,15,0,0,0,";
        Assert.That(signature, Is.EqualTo(expectedSignature), "Row signature does not match expected value.");
    }

    [Test]
    public void GenerateMonstersAndTreasures_Should_Create_Unique_Rows()
    {
        // Arrange
        const int height = 5;
        const int width = 5;
        var monstersGrid = new int[height][];
        var treasuresGrid = new int[height][];

        for (var i = 0; i < height; i++)
        {
            monstersGrid[i] = new int[width];
            treasuresGrid[i] = new int[width];
        }

        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(monstersGrid, treasuresGrid);

        // Assert
        HashSet<string> uniqueSignatures = [];
        for (var y = 0; y < height; y++)
        {
            var signature = Algorithms.GT.GetRowSignature(monstersGrid[y], treasuresGrid[y]);
            Assert.That(uniqueSignatures.Add(signature), Is.True, $"Row {y} is not unique.");
        }
    }
}