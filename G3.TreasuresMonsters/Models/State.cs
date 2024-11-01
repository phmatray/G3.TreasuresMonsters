namespace G3.TreasuresMonsters.Models;

/// <summary>
/// State object : contains everything related to a game's current state
/// </summary>
public class State
{
    public State(
        int[] heroPos,
        int heroHealth,
        int heroScore,
        int[][] monsters,
        int[][] treasures,
        int nbHint,
        int nbLevel)
    {
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
    public int[] HeroPos { get; private set; }
    
    /// <summary>
    /// Current health of the hero, between 0 and 100
    /// </summary>
    public int HeroHealth { get; private set; }
    
    /// <summary>
    /// Current score of the hero
    /// </summary>
    public int HeroScore { get; private set; }
    
    /// <summary>
    /// Monsters in the current level
    /// </summary>
    public int[][] Monsters { get; private set; }
    
    /// <summary>
    /// Treasures in the current level
    /// </summary>
    public int[][] Treasures { get; private set; }
    
    /// <summary>
    /// Number of hints available to the player
    /// </summary>
    public int NbHint { get; private set; }
    
    /// <summary>
    /// Current level number, starting at 1 and going up
    /// </summary>
    public int NbLevel { get; private set; }
    
    // Additional properties
    public int DungeonScoreToBeat { get; private set; }
    public MovementConstraint HeroMoveConstraint { get; private set; } = MovementConstraint.None;
    public int HeroX => HeroPos[0];
    public int HeroY => HeroPos[1];
    public bool HeroIsAlive => HeroHealth > 0;
    public bool HeroIsDead => HeroHealth <= 0;
    
    public void MoveHeroLeft()
    {
        HeroPos[0]--;
        HeroMoveConstraint = MovementConstraint.Right;
    }
    
    public void MoveHeroRight()
    {
        HeroPos[0]++;
        HeroMoveConstraint = MovementConstraint.Left;
    }
    
    public void MoveHeroDown()
    {
        HeroPos[1]++;
        HeroMoveConstraint = MovementConstraint.None;
    }

    public void IncreaseHeroHealth(int value)
    {
        HeroHealth = Math.Min(100, HeroHealth + value);
    }

    public void DecreaseHeroHealth(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value must be positive");

        if (HeroHealth - value >= 0)
        {
            HeroHealth -= value;
        }
        else
        {
            HeroHealth = 0;
        }
    }

    public void IncreaseHeroScore(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value must be positive");

        HeroScore += value;
    }

    public void AddHint()
    {
        NbHint++;
    }

    public void DecreaseHint()
    {
        if (NbHint > 0)
        {
            NbHint--;
        }
    }
    
    public void IncreaseCurrentLevel()
    {
        NbLevel++;
    }
    
    public void InitializeDungeon()
    {
        IncreaseHeroHealth(50);
        Monsters = new int[Constants.DungeonHeight][];
        Treasures = new int[Constants.DungeonHeight][];
        
        // Initialize the monsters and treasures
        for (int i = 0; i < Constants.DungeonHeight; i++)
        {
            Monsters[i] = new int[Constants.DungeonWidth];
            Treasures[i] = new int[Constants.DungeonWidth];
        }

        Algorithms.GT.GenerateMonstersAndTreasures(Monsters, Treasures);
        Algorithms.DC.SortLevel(Monsters, Treasures); // Trier le niveau après la génération
        
        // Initialize the hero's position
        HeroPos = [Constants.DungeonWidth / 2, 0];

        // If the hero is on a monster or a treasure, move the obstacle to the first free cell on the top row
        if (Monsters[HeroY][HeroX] > 0 || Treasures[HeroY][HeroX] > 0)
        {
            for (int x = 0; x < Constants.DungeonWidth; x++)
            {
                if (Monsters[0][x] != 0 || Treasures[0][x] != 0)
                {
                    continue;
                }
                
                Monsters[0][x] = Monsters[HeroY][HeroX];
                Treasures[0][x] = Treasures[HeroY][HeroX];
                Monsters[HeroY][HeroX] = 0;
                Treasures[HeroY][HeroX] = 0;
                break;
            }
        }
        
        // TODO: Calculate the score to beat
        DungeonScoreToBeat = Algorithms.GS.GreedySolution(this);
    }
}
