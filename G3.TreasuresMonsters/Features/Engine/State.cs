using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Features.Engine;

/// <summary>
/// State object : contains everything related to a game's current state
/// </summary>
public class State
{
    public Hero Hero { get; } = new();
    
    /// <summary>
    /// Monsters in the current level, 0 if no monster
    /// </summary>
    public int[][] Monsters { get; } = [];
    
    /// <summary>
    /// Treasures in the current level, 0 if no treasure
    /// </summary>
    public int[][] Treasures { get; } = [];

    /// <summary>
    /// Current level number, starting at 1 and going up
    /// </summary>
    public int NbLevel { get; set; } = 1;
}
