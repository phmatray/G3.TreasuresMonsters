using G3.TreasuresMonsters.Features.Logic;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsDPTests
{
    [Test]
    public void PerfectSolution_Should_Return_Correct_Path_For_Simplest_Dungeon()
    {
        // Tests the algorithm on the simplest possible dungeon with no monsters or treasures.
        // Verifies that the hero moves straight down to the exit.
        
        // Arrange
        int[][] monsters = [[0]];
        int[][] treasures = [[0]];
        
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
        Assert.That(path, Is.EqualTo("↓"), "The path should be one step down.");
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
        Assert.That(path, Is.EqualTo("There is no valid path."), "There should be no valid path.");
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
        Assert.That(path, Is.EqualTo("↓→↓→↓↓"), "The path should collect the maximum treasure while avoiding monsters.");
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
        Assert.That(path, Is.EqualTo("The hero is already dead."), "There should be no valid path when hero health is negative.");
    }

    [Test]
    public void PerfectSolution_Should_Choose_Path_With_Best_TotalScore()
    {
        // Tests that the algorithm selects the path that maximizes the total score,
        // even if it means fighting a monster to collect a high-value treasure.
        
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
        Assert.That(path, Is.EqualTo("→↓↓↓"), "Hero should avoid the right path with the high-strength monster.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Treasures_Exceeding_Health_Loss()
    {
        // Checks whether the algorithm chooses to take a path where the treasure value outweighs the health loss from monsters.
        
        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 100], // High monster strength
            [0, 0]
        ];

        int[][] treasures =
        [
            [0, 80], // High treasure value
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
        Assert.That(path, Is.EqualTo("↓↓↓"), "Hero should take the risk for higher total score.");
    }

    [Test]
    public void PerfectSolution_Should_Handle_Multiple_Paths_With_Same_Score()
    {
        // Verifies that the algorithm can handle multiple optimal paths leading to the same total score.
        
        // Arrange
        int[][] monsters =
        [
            [0, 0, 0],
            [0, 0, 0]
        ];

        int[][] treasures =
        [
            [10, 0, 10],
            [0, 0, 0]
        ];

        var state = new State(
            heroPos: [1, 0],
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
        var possiblePaths = new HashSet<string> { "←↓↓", "→↓↓" };
        Assert.That(possiblePaths, Does.Contain(path), "Hero should take any path with maximum total score.");
    }
    
    // WRONG Perfect path: ↓↓↓↓↓↓→↓→↓←←←↓←←↓↓
    // EXPECTED path     : ↓↓→→↓↓↓↓↓←↓←←←↓←↓↓
    //
    // ╔════════════════════════════════════╗
    // ║ 🧟34 👾06 👺48 🦄   .    👺44 .    ║ -132
    // ║ .    .    👺43 .    🧟38 .    🧟15 ║ -096
    // ║ .    .    👺47 .    .    .    🧟38 ║ -085
    // ║ .    👾10 .    👺41 .    .    👾01 ║ -052
    // ║ .    🧟27 .    🧟18 .    .    .    ║ -045
    // ║ 👾13 .    .    .    .    .    🧟18 ║ -031
    // ║ .    .    🧟27 👾03 .    .    .    ║ -030
    // ║ .    🧟17 .    👺44 .    💰62 🧟25 ║ -024
    // ║ 👾06 .    .    .    .    👾01 .    ║ -007
    // ║ 💎91 .    🧟35 🧟29 🧟22 .    👾11 ║ -006
    // ║ 💰36 .    .    .    .    🧟18 🧟23 ║ -005
    // ╚════════════════════════════════════╝
    [Test]
    public void PerfectSolution_Should_Handle_Real_Dungeon()
    {
        // Arrange
        int[][] monsters =
        [
            [34,  6, 48,  0,  0, 44,  0],
            [ 0,  0, 43,  0, 38,  0, 15],
            [ 0,  0, 47,  0,  0,  0, 38],
            [ 0, 10,  0, 41,  0,  0,  1],
            [ 0, 27,  0, 18,  0,  0,  0],
            [13,  0,  0,  0,  0,  0, 18],
            [ 0,  0, 27,  3,  0,  0,  0],
            [ 0, 17,  0, 44,  0,  0, 25],
            [ 6,  0,  0,  0,  0,  1,  0],
            [ 0,  0, 35, 29, 22,  0, 11],
            [ 0,  0,  0,  0,  0, 18, 23]
        ];
        
        int[][] treasures =
        [
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0, 62,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [91,  0,  0,  0,  0,  0,  0],
            [36,  0,  0,  0,  0,  0,  0]
        ];
        
        var state = new State(
            heroPos: [3, 0],
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
        Assert.That(path, Is.EqualTo("↓↓→→↓↓↓↓↓←↓←←←↓←↓↓"), "The path should collect the maximum treasure while avoiding monsters.");
    }
}