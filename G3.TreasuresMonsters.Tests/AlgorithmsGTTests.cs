using System.Reflection;
using G3.TreasuresMonsters.Features.Logic;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsGTTests
{
    private int[][] _monsters = [];
    private int[][] _treasures = [];
    private int _height;
    private int _width;

    [SetUp]
    public void SetUp()
    {
        Algorithms.SetSeed(42);
        _height = 10;
        _width = 10;
        _monsters = new int[_height][];
        _treasures = new int[_height][];

        for (int i = 0; i < _height; i++)
        {
            _monsters[i] = new int[_width];
            _treasures[i] = new int[_width];
        }
    }
    
    [Test]
    //une case du plateau a 1 chance sur 6 de contenir un tresor
    public void GenerateMonstersAndTreasures_ShouldDistributeTreasuresWithCorrectProbability()
    {
        // Arrange
        _height = 1000;
        _width = 6;
        _monsters = new int[_height][];
        _treasures = new int[_height][];

        for (int i = 0; i < _height; i++)
        {
            _monsters[i] = new int[_width];
            _treasures[i] = new int[_width];
        }

        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(_monsters, _treasures);

        // Assert
        int totalCells = _height * _width;
        int treasureCells = _treasures.Sum(row => row.Count(x => x > 0));

        double treasureProbability = (double)treasureCells / totalCells;

        Assert.That(treasureProbability, Is.EqualTo(1.0 / 6).Within(0.05), "The probability of a cell containing a treasure should be approximately 1/6.");
    }
        
        
    [Test]
    public void GenerateMonstersAndTreasures_ShouldGenerateValidGrid()
    {
        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(_monsters, _treasures);

        // Assert
        for (int y = 0; y < _height; y++)
        {
            int monsterCount = _monsters[y].Count(x => x > 0);
            int treasureCount = _treasures[y].Count(x => x > 0);
            int totalMonsterStrength = _monsters[y].Sum();
            int totalTreasureValue = _treasures[y].Sum();

            Assert.That(monsterCount, Is.GreaterThanOrEqualTo(2), "Each row should have at least 2 monsters.");
            Assert.That(treasureCount, Is.LessThanOrEqualTo(2), "Each row should have at most 2 treasures.");
            Assert.That(totalTreasureValue, Is.LessThanOrEqualTo(totalMonsterStrength), "Total treasure value must not exceed total monster strength.");
        }
    }

    [Test]
    public void GenerateMonstersAndTreasures_ShouldNotHaveDuplicatePositionsForMonstersAndTreasures()
    {
        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(_monsters, _treasures);

        // Assert
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Assert.That(_monsters[y][x] > 0 && _treasures[y][x] > 0, Is.False, "A cell should not contain both a monster and a treasure.");
            }
        }
    }

    [Test]
    public void GenerateMonstersAndTreasures_ShouldEnsurePlayerStartPositionIsEmpty()
    {
        // Arrange
        int playerStartX = _width / 2;
        int playerStartY = 0;

        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(_monsters, _treasures);

        // Assert
        Assert.That(_monsters[playerStartY][playerStartX], Is.EqualTo(0), "Player start position should not contain a monster.");
        Assert.That(_treasures[playerStartY][playerStartX], Is.EqualTo(0), "Player start position should not contain a treasure.");
    }

    [Test]
    public void GenerateMonstersAndTreasures_ShouldDistributeMonstersAndTreasuresWithCorrectProbability()
    {
        // Arrange
        _height = 1000;
        _width = 1000;
        _monsters = new int[_height][];
        _treasures = new int[_height][];

        for (int i = 0; i < _height; i++)
        {
            _monsters[i] = new int[_width];
            _treasures[i] = new int[_width];
        }

        // Act
        Algorithms.GT.GenerateMonstersAndTreasures(_monsters, _treasures);

        // Assert
        int totalCells = _height * _width;
        int monsterCells = _monsters.Sum(row => row.Count(x => x > 0));
        int treasureCells = _treasures.Sum(row => row.Count(x => x > 0));
        int emptyCells = totalCells - monsterCells - treasureCells;

        double monsterProbability = (double)monsterCells / totalCells;
        double treasureProbability = (double)treasureCells / totalCells;
        double emptyProbability = (double)emptyCells / totalCells;

        Assert.That(monsterProbability, Is.EqualTo(1.0 / 3).Within(0.05), "The probability of a cell containing a monster should be approximately 1/3.");
        Assert.That(treasureProbability, Is.EqualTo(1.0 / 6).Within(0.05), "The probability of a cell containing a treasure should be approximately 1/6.");
        Assert.That(emptyProbability, Is.EqualTo(1.0 / 2).Within(0.05), "The probability of a cell being empty should be approximately 1/2.");
    }

    [Test]
    public void GenerateRow_ShouldGenerateValidRow()
    {
        // Arrange
        int[] monstersRow = new int[_width];
        int[] treasuresRow = new int[_width];

        // Act
        typeof(Algorithms.GT)
            .GetMethod("GenerateRow", BindingFlags.NonPublic | BindingFlags.Static)?
            .Invoke(null, [monstersRow, treasuresRow]);

        // Assert
        int monsterCount = monstersRow.Count(x => x > 0);
        int treasureCount = treasuresRow.Count(x => x > 0);
        int totalMonsterStrength = monstersRow.Sum();
        int totalTreasureValue = treasuresRow.Sum();

        Assert.That(monsterCount, Is.GreaterThanOrEqualTo(2), "Row should have at least 2 monsters.");
        Assert.That(treasureCount, Is.LessThanOrEqualTo(2), "Row should have at most 2 treasures.");
        Assert.That(totalTreasureValue, Is.LessThanOrEqualTo(totalMonsterStrength), "Total treasure value must not exceed total monster strength.");
    }
}