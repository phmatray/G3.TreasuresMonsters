namespace G3.TreasuresMonsters.Models;

/// <summary>
/// State object : contains everything related to a game's current state
/// </summary>
public class State
{
    public State(int[] heroPos, int heroHealth, int heroScore, int[][] monsters, int[][] treasures, int nbHint, int nbLevel) {
        HeroPos = heroPos;
        HeroHealth = heroHealth;
        HeroScore = heroScore;
        Monsters = monsters;
        Treasures = treasures;
        NbHint = nbHint;
        NbLevel = nbLevel;
    }
    
    /// <summary>
    /// Current position of the hero in the level
    /// </summary>
    public int[] HeroPos { get; set; }
    
    /// <summary>
    /// Current health of the hero, between 0 and 100
    /// </summary>
    public int HeroHealth { get; set; }     // Santé du héros
    
    /// <summary>
    /// Current score of the hero
    /// </summary>
    public int HeroScore { get; set; }      // Score du héros
    
    /// <summary>
    /// Monsters in the current level
    /// </summary>
    public int[][] Monsters { get; set; }   // Tableau 2D des monstres (force), 0 si pas de monstre
    
    /// <summary>
    /// Treasures in the current level
    /// </summary>
    public int[][] Treasures { get; set; }  // Tableau 2D des trésors (valeur), 0 si pas de trésor
    
    /// <summary>
    /// Number of hints available to the player
    /// </summary>
    public int NbHint { get; set; }
    
    /// <summary>
    /// Current level number, starting at 1 and going up
    /// </summary>
    public int NbLevel { get; set; }

    public int HeroX => HeroPos[0];
    
    public int HeroY => HeroPos[1];
}
