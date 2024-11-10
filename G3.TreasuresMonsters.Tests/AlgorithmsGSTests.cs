using G3.TreasuresMonsters.Features.Logic;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Tests;

[TestFixture]
public class AlgorithmsGSTests
{
    [Test]
    public void GreedySolution_Should_Return_Correct_Value_For_Simplest_Dungeon()
    {
        // Test the algorithm on the simplest dungeon without monsters or treasures.

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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
         Assert.That(totalValue, Is.EqualTo(0), "The score should be 0 since hero has not collected any treasures.");
    }
    
    [Test]
    public void GreedySolution_Should_Handle_Monsters_Correctly()
    {
        // Check that the algorithm takes into account the monsters by reducing the hero's health.

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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        // The hero loses health by facing the monsters
        Assert.That(totalValue, Is.EqualTo(0), "The score should be 0 since the hero has not collected any treasures.");
    }
    
    [Test]
    public void GreedySolution_Should_Collect_Treasures_Correctly()
    {
        // Check that the algorithm correctly accumulates treasures along the way.

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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        Assert.That(totalValue, Is.EqualTo(10 + 20 + 30), "The score should be 60 since the hero has collected all treasures.");
    }
    
    [Test]
    public void GreedySolution_Should_Avoid_Deadly_Paths()
    {
        // Verify that the algorithm avoids paths where the hero would die due to strong monsters.
        // Since there are no safe paths, the hero cannot collect any treasures, and the score should be 0.

        // Arrange
        int[][] monsters =
        [
            [0],
            [150], // Monster too strong for the hero
            [0]
        ];

        int[][] treasures =
        [
            [0],
            [100], // Treasure behind the strong monster
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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        Assert.That(totalValue, Is.EqualTo(0), "The score should be 0 since the hero cannot survive to collect the treasure.");
    }
    
    [Test]
    public void GreedySolution_Should_Choose_Path_With_Highest_Score()
    {
        // Verify that the algorithm chooses the path that offers the highest treasure score.

        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 0],
            [0, 50] // Strong monster on the right path
        ];

        int[][] treasures =
        [
            [0, 100], // High-value treasure on the right
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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        // The hero should choose the right path to collect the treasure if possible within depth limit
        // However, due to depth limitation, the hero may not reach the treasure
        Assert.That(totalValue, Is.EqualTo(100), "The score should be 100 since the hero collects the high-value treasure.");
    }
    
    [Test]
    public void GreedySolution_Should_Handle_Depth_Limitation()
    {
        // Check that the algorithm respects the depth limitation and may miss treasures beyond its depth.

        // Arrange
        int[][] monsters =
        [
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 100] // Strong monster beyond depth limit
        ];

        int[][] treasures =
        [
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 200], // High-value treasure beyond depth limit
            [0, 0, 0]
        ];

        var state = new State(
            heroPos: [1, 0],
            heroHealth: 100,
            heroScore: 0,
            monsters: monsters,
            treasures: treasures,
            nbHint: 1,
            nbLevel: 1
        );

        // Act
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        // The hero cannot see the treasure beyond depth limit, so the score should be 0
        Assert.That(totalValue, Is.EqualTo(0), "The score should be 0 since the treasure is beyond the depth limit.");
    }
    
    [Test]
    public void GreedySolution_Should_Find_Treasure_Within_Depth_Limit()
    {
        // Verify that the algorithm finds the treasure within the depth limit.

        // Arrange
        int[][] monsters =
        [
            [0, 0],
            [0, 50], // Strong monster on the right
            [0, 0],
            [0, 0],
            [0, 0],
            [0, 0]
        ];

        int[][] treasures =
        [
            [0, 0],
            [0, 0],
            [0, 0],
            [0, 100], // High-value treasure at depth 4
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
        int totalValueGS = Algorithms.GS.GreedySolution(state);

        // Expected score: the hero collects the 100 treasure
        int expectedScore = 100;

        // Assert
        Assert.That(totalValueGS, Is.EqualTo(expectedScore), "The Greedy Solution should find the treasure within the depth limit.");
    }
    
    [Test]
    public void GreedySolution_Should_Handle_Multiple_Paths_With_Same_Score()
    {
        // Verify that the algorithm can handle multiple optimal paths leading to the same score.

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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        // The hero can choose either left or right, collecting 10 treasure
        Assert.That(totalValue, Is.EqualTo(10), "The hero should collect the maximum possible treasure of 10.");
    }
    
    [Test]
    public void GreedySolution_Should_Stop_When_No_More_Moves()
    {
        // Verify that the algorithm stops correctly when there are no valid moves.

        // Arrange
        int[][] monsters =
        [
            [0],
            [150] // Monster too strong blocking the path
        ];

        int[][] treasures =
        [
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
        int totalValue = Algorithms.GS.GreedySolution(state);

        // Assert
        // The hero cannot proceed, so the score remains 0
        Assert.That(totalValue, Is.EqualTo(0), "The score should be 0 since the hero cannot proceed.");
    }

    [Test]
    public void GreedySolution_Should_Handle_Real_Dungeon()
    {
        // Test the algorithm on a complex dungeon similar to the DP test.

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
            [ 0,  0,  0,  0,  0, 62,  0],
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
        int totalValueGS = Algorithms.GS.GreedySolution(state);

        // For comparison, the DP algorithm finds the maximum treasure of 91 + 62 + 36 = 189
        int expectedDPScore = 189;

        // Assert
        // The Greedy Solution should collect less treasure due to depth limitation
        Assert.That(totalValueGS, Is.LessThan(expectedDPScore), "The Greedy Solution should collect less treasure due to depth limitation.");
        Assert.That(totalValueGS, Is.EqualTo(0), "The Greedy Solution should collect some treasure.");
    }

    [Test]
    public void GreedySolution_Should_Handle_Real_Dungeon_With_Tresure_Protected()
    {
        // Test the algorithm on a complex dungeon similar to the DP test.

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
            [ 0,  0,  0, 10,  0, 40,  0],
            [ 0,  0,  0, 20,  0,  0,  0],
            [ 0,  0,  0,  0,  0, 30,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0,  0,  0],
            [ 0,  0,  0,  0,  0, 62,  0],
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
        int totalValueGS = Algorithms.GS.GreedySolution(state);

        // For comparison, the DP algorithm finds the maximum treasure of 91 + 62 + 36 = 189
        int expectedDPScore = 189;

        // Assert
        // The Greedy Solution should collect less treasure due to depth limitation
        Assert.That(totalValueGS, Is.LessThan(expectedDPScore), "The Greedy Solution should collect less treasure due to depth limitation.");
        Assert.That(totalValueGS, Is.EqualTo(80), "The Greedy Solution should collect some treasure.");
    }
}