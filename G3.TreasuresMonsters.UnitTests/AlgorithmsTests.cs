using G3.TreasuresMonsters.Models;
using static G3.TreasuresMonsters.Logic.Algorithms;

namespace G3.TreasuresMonsters.UnitTests;

public class AlgorithmsTests
{
    private const int Width = 7;
    private const int Height = 5;

    [Fact]
    public void GenerateMonstersAndTreasures_ShouldGenerateValidLevel()
    {
        // Arrange
        int[][] monsters = CreateEmptyGrid();
        int[][] treasures = CreateEmptyGrid();

        // Act
        GT.GenerateMonstersAndTreasures(monsters, treasures);

        // Assert
        for (int y = 0; y < Height; y++)
        {
            int monsterCount = 0;
            int treasureCount = 0;
            int totalMonsterStrength = 0;
            int totalTreasureValue = 0;

            for (int x = 0; x < Width; x++)
            {
                if (monsters[y][x] > 0)
                {
                    monsterCount++;
                    totalMonsterStrength += monsters[y][x];
                    Assert.True(monsters[y][x] >= 1 && monsters[y][x] <= 50);
                }

                if (treasures[y][x] > 0)
                {
                    treasureCount++;
                    totalTreasureValue += treasures[y][x];
                    Assert.True(treasures[y][x] >= 1 && treasures[y][x] <= 99);
                }

                // Ensure a cell is not both monster and treasure
                Assert.False(monsters[y][x] > 0 && treasures[y][x] > 0);
            }

            // Validate constraints
            Assert.True(monsterCount >= 2);
            Assert.True(treasureCount <= 2);
            Assert.True(totalTreasureValue <= totalMonsterStrength);
        }
    }

    [Fact]
    public void SortLevel_ShouldSortRowsCorrectly()
    {
        // Arrange
        int[][] monsters = CreateEmptyGrid();
        int[][] treasures = CreateEmptyGrid();

        // Fill the grid with predefined values to test sorting
        // Row 0: High negative row value (more monsters)
        monsters[0][0] = 30;
        monsters[0][1] = 20;

        // Row 1: Zero row value
        monsters[1][0] = 10;
        treasures[1][1] = 10;

        // Row 2: Positive row value (more treasures)
        treasures[2][0] = 40;
        treasures[2][1] = 20;

        // Row 3: Negative row value
        monsters[3][0] = 15;
        monsters[3][1] = 5;

        // Row 4: High positive row value
        treasures[4][0] = 50;
        treasures[4][1] = 30;

        // Act
        DC.SortLevel(monsters, treasures);

        // Assert
        // Compute the row values after sorting
        var rowValues = new List<int>();
        for (int y = 0; y < Height; y++)
        {
            int totalTreasure = 0;
            int totalMonsters = 0;
            for (int x = 0; x < Width; x++)
            {
                totalTreasure += treasures[y][x];
                totalMonsters += monsters[y][x];
            }

            rowValues.Add(totalTreasure - totalMonsters);
        }

        // Check if the row values are in ascending order
        for (int i = 1; i < rowValues.Count; i++)
        {
            Assert.True(rowValues[i - 1] <= rowValues[i]);
        }
    }

    [Fact]
    public void GreedySolution_ShouldReturnNonOptimalScore()
    {
        // Arrange
        int[] heroPos = [0, 0];
        int[][] monsters = CreateEmptyGrid();
        int[][] treasures = CreateEmptyGrid();

        // Create a grid where the optimal path is not the greedy one
        // Place a high-value treasure behind a monster with high strength

        // Monster blocking the treasure
        monsters[0][1] = 40;
        // High-value treasure
        treasures[1][1] = 80;

        State state = new State(heroPos, 100, 0, monsters, treasures, 0, 1);

        // Act
        int greedyScore = GS.GreedySolution(state);

        // Assert
        // The greedy algorithm should avoid the high-strength monster
        // and not collect the high-value treasure
        int expectedGreedyScore = 100; // Health remains the same, no treasures collected
        Assert.Equal(expectedGreedyScore, greedyScore);
    }

    [Fact]
    public void PerfectSolution_ShouldReturnOptimalPath()
    {
        // Arrange
        int[] heroPos = [0, 0];
        int[][] monsters = CreateEmptyGrid();
        int[][] treasures = CreateEmptyGrid();

        // Place monsters and treasures
        monsters[0][1] = 20;
        treasures[1][1] = 50;
        monsters[1][2] = 10;
        treasures[2][2] = 30;

        State state = new State(heroPos, 100, 0, monsters, treasures, 0, 1);

        // Act
        string perfectPath = DP.PerfectSolution(state);

        // Assert
        // Expected path is "DRD" to collect both treasures
        Assert.Equal("DRD", perfectPath);

        // Calculate the score following the perfect path
        int health = 100;
        int treasuresCollected = 0;
        int x = state.HeroX;
        int y = state.HeroY;
        foreach (char move in perfectPath)
        {
            switch (move)
            {
                case 'D':
                    y += 1;
                    break;
                case 'L':
                    x -= 1;
                    break;
                case 'R':
                    x += 1;
                    break;
            }

            if (state.Monsters[y][x] > 0)
            {
                health -= state.Monsters[y][x];
            }

            if (state.Treasures[y][x] > 0)
            {
                treasuresCollected += state.Treasures[y][x];
            }
        }

        int expectedScore = health + treasuresCollected;
        var memo = new Dictionary<(int x, int y, int health), (int score, string path)>();
        var result = DP.DP_Search(state.HeroX, state.HeroY, state.HeroHealth, state, memo);

        Assert.Equal(expectedScore, result.score);
    }

    // Helper method to create an empty grid
    private int[][] CreateEmptyGrid()
    {
        int[][] grid = new int[Height][];
        for (int i = 0; i < Height; i++)
        {
            grid[i] = new int[Width];
        }

        return grid;
    }
}
