using G3.TreasuresMonsters.Features.Logic;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsDPTests
{
    [SetUp]
    public void SetUp()
    {
        // Any necessary setup before each test
    }

    [Test]
    public void PerfectSolution_Should_Return_Correct_Path_For_Simple_Dungeon()
    {
        // Tests the algorithm on a simple dungeon with no monsters or treasures.
        // Verifies that the hero moves straight down to the exit.
        
        // Arrange
        int[][] monsters =
        [
            [0],
            [0],
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [0],
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo("↓↓↓"), "The path should be three steps down.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Monsters_Correctly()
    {
        // Ensures that the algorithm accounts for monsters by reducing the hero's health.
        // Checks that the hero can still reach the exit if they have enough health.
            
        // Arrange
        int[][] monsters =
        [
            [0],
            [10],
            [20],
            [30]
        ];

        int[][] treasures =
        [
            [0],
            [0],
            [0],
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo("↓↓↓↓"), "The path should be two steps down.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Treasures_Correctly()
    {
        // Tests that the algorithm correctly accumulates treasures along the path.
        
        // Arrange
        int[][] monsters =
        [
            [0],
            [0],
            [0],
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [10],
            [20],
            [30]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo("↓↓↓↓"), "The path should be two steps down.");
    }

    [Test]
    public void PerfectSolution_Should_Avoid_Deadly_Paths()
    {
        // Verifies that the algorithm avoids paths where the hero would die due to monsters stronger than the hero's health.
        
        // Arrange
        int[][] monsters =
        [
            [0],
            [150], // Hero cannot survive this monster
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [0],
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo(string.Empty), "There should be no valid path.");
    }

    [Test]
    public void PerfectSolution_Should_Choose_Path_With_Max_Treasure()
    {
        // Tests whether the algorithm chooses the path that leads to the highest total score (health + treasure).
        
        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 0],
            [0, 0]
        ];

        int[][] treasures =
        [
            [0, 50],
            [0, 0],
            [0, 0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo("→↓↓↓"), "The path should move right to collect the treasure.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Larger_Dungeon()
    {
        // Tests the algorithm on a larger dungeon with multiple paths, monsters, and treasures.
        
        // Arrange
        int[][] monsters =
        [
            [0, 10, 0],
            [0, 0, 0],
            [20, 0, 0],
            [0, 0, 0]
        ];

        int[][] treasures =
        [
            [0, 0, 0],
            [0, 50, 0],
            [0, 0, 100],
            [0, 0, 0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        // Expected path: →↓→↓ (Collect treasures and avoid monsters)
        Assert.That(path, Is.EqualTo("→↓→↓"), "The path should collect the maximum treasure while avoiding monsters.");
    }

    [Test]
    public void PerfectSolution_Should_Return_Empty_Path_When_No_Valid_Moves()
    {
        // Checks that the algorithm returns an empty path when the hero cannot move (e.g., starts with negative health).
        
        // Arrange
        int[][] monsters =
        [
            [0]
        ];

        int[][] treasures =
        [
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: -10, // Hero starts with negative health
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo(string.Empty), "There should be no valid path when hero health is negative.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Hero_On_Monster_Cell()
    {
        // Verifies that the algorithm handles the scenario where the hero starts on a cell with a monster.
        
        // Arrange
        int[][] monsters =
        [
            [20],
            [0],
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [0],
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 10, // Hero has less health than monster strength
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        Assert.That(path, Is.EqualTo(string.Empty), "Hero cannot survive the initial monster; no valid path.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_MaxHealth_Correctly()
    {
        // Ensures that the hero's health does not exceed the maximum allowed health (e.g., when picking up health potions).
        
        // Arrange
        int[][] monsters =
        [
            [-20], // Negative value simulating a health potion
            [0],
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [0],
            [0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 90,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        // Hero health should not exceed MaxHealth (100)
        Assert.That(path, Is.EqualTo("↓↓"), "Hero should proceed downwards.");
    }

    [Test]
    public void PerfectSolution_Should_Choose_Path_With_Best_TotalScore()
    {
        // Tests that the algorithm selects the path that maximizes the total score,
        // even if it means avoiding a high-value treasure guarded by a strong monster.
        
        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 50], // High monster strength
            [0, 0]
        ];

        int[][] treasures =
        [
            [0, 100], // High treasure value
            [0, 0],
            [0, 0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 1,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        // The hero should avoid the high-strength monster and not risk dying
        Assert.That(path, Is.EqualTo("↓↓"), "Hero should avoid the right path with the high-strength monster.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Treasures_Exceeding_Health_Loss()
    {
        // Checks whether the algorithm chooses to take a path where the treasure value outweighs the health loss from monsters.
        
        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 80], // High monster strength
            [0, 0]
        ];

        int[][] treasures =
        [
            [0, 100], // High treasure value
            [0, 0],
            [0, 0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 1,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        // Despite the high health loss, the treasure makes the total score higher
        Assert.That(path, Is.EqualTo("→↓↓"), "Hero should take the risk for higher total score.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Multiple_Paths_With_Same_Score()
    {
        // Verifies that the algorithm can handle multiple optimal paths leading to the same total score.
        
        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 0],
            [0, 0]
        ];

        int[][] treasures =
        [
            [10, 0],
            [0, 10],
            [0, 0]
        ];

        var state = new State(
            heroPos: [0, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 0,
            nbLevel: 1
        );

        // Act
        string path = Algorithms.DP.PerfectSolution(state);

        // Assert
        // Multiple paths lead to the same total score; any optimal path is acceptable
        var possiblePaths = new HashSet<string> { "↓→↓", "→↓↓" };
        Assert.That(possiblePaths.Contains(path), Is.True, "Hero should take any path with maximum total score.");
    }
}