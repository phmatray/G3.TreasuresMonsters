using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Features.Engine;

/// <summary>
/// State object : contains everything related to a game's current state
/// </summary>
public class State
{
    public Hero Hero { get; } = new();
    
    public Dungeon Dungeon { get; set; } = new();
    
    /// <summary>
    /// Current level number, starting at 1 and going up
    /// </summary>
    public int NbLevel { get; set; } = 1;
}
