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
    public int DungeonHeight => Monsters.Length;
    public int DungeonWidth => Monsters[0].Length;
    
    public State Copy()
    {
        int[] heroPos = [HeroPos[0], HeroPos[1]];
        var monsters = CopyArray(Monsters);
        var treasures = CopyArray(Treasures);
        
        return new State(
            heroPos,
            HeroHealth,
            HeroScore,
            monsters,
            treasures,
            NbHint,
            NbLevel
        );
    }
    
    public int[][] CopyArray(int[][] array)
    {
        var copy = new int[array.Length][];
        
        for (int i = 0; i < array.Length; i++)
        {
            copy[i] = new int[array[i].Length];
        }
        
        for (int y = 0; y < array.Length; y++)
        {
            for (int x = 0; x < array[y].Length; x++)
            {
                copy[y][x] = array[y][x];
            }
        }
        
        return copy;
    }
    
    public void ApplyMove(string move)
    {
        switch (move)
        {
            case Constants.MoveDown:
                MoveDown();
                break;
            case Constants.MoveLeft:
                MoveLeft();
                break;
            case Constants.MoveRight:
                MoveRight();
                break;
            default:
                throw new ArgumentException("Invalid move");
        }
    }
    
    public bool CanMoveLeft()
    {
        if (HeroMoveConstraint == MovementConstraint.Left)
        {
            return false;
        }
        
        return HeroX - 1 >= 0;
    }
    
    public bool CanMoveRight()
    {
        if (HeroMoveConstraint == MovementConstraint.Right)
        {
            return false;
        }
        
        return HeroX + 1 < DungeonWidth;
    }
    
    public CellResolution MoveDown()
    {
        HeroPos[1]++;
        HeroMoveConstraint = MovementConstraint.None;

        if (HeroY >= DungeonHeight)
        {
            // End of the level
            return new CellResolution(CellResolutionType.EndOfLevel, 0);
        }

        return ResolveCell();
    }
    
    public CellResolution MoveLeft()
    {
        if (!CanMoveLeft())
            throw new InvalidOperationException("Cannot move left");
        
        HeroPos[0]--;
        HeroMoveConstraint = MovementConstraint.Right;
        return ResolveCell();
    }
    
    public CellResolution MoveRight()
    {
        if (!CanMoveRight())
            throw new InvalidOperationException("Cannot move right");
        
        HeroPos[0]++;
        HeroMoveConstraint = MovementConstraint.Left;
        return ResolveCell();
    }
    
    public CellResolution ResolveCell()
    {
        // Ensure the hero's position is within bounds
        if (HeroY < 0 || HeroY >= DungeonHeight || HeroX < 0 || HeroX >= DungeonWidth)
            throw new InvalidOperationException("Hero is out of dungeon bounds");
        
        // Check for monster
        var monsterStrength = Monsters[HeroY][HeroX];
        if (monsterStrength > 0)
        {
            DecreaseHeroHealth(monsterStrength);
            Monsters[HeroY][HeroX] = 0;
            return new CellResolution(CellResolutionType.Monster, monsterStrength);
        }

        // Check for treasure
        var treasureValue = Treasures[HeroY][HeroX];
        if (treasureValue > 0)
        {
            IncreaseHeroScore(treasureValue);
            Treasures[HeroY][HeroX] = 0;
            return new CellResolution(CellResolutionType.Treasure, treasureValue);
        }

        return new CellResolution(CellResolutionType.Empty, 0);
    }

    public void IncreaseHeroHealth(int value)
    {
        HeroHealth = Math.Min(100, HeroHealth + value);
    }

    public void DecreaseHeroHealth(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value must be positive");

        HeroHealth = Math.Max(0, HeroHealth - value);
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
        
        // Initialize the monsters and treasures
        Monsters = new int[Constants.InitialDungeonHeight][];
        Treasures = new int[Constants.InitialDungeonHeight][];
        
        for (int i = 0; i < Constants.InitialDungeonHeight; i++)
        {
            Monsters[i] = new int[Constants.InitialDungeonWidth];
            Treasures[i] = new int[Constants.InitialDungeonWidth];
        }

        Algorithms.GT.GenerateMonstersAndTreasures(Monsters, Treasures);
        Algorithms.DC.SortLevel(Monsters, Treasures); // Trier le niveau après la génération
        
        // Initialize the hero's position
        HeroPos = [DungeonWidth / 2, 0];

        // If the hero is on a monster or a treasure, move the obstacle to the first free cell on the top row
        if (Monsters[HeroY][HeroX] > 0 || Treasures[HeroY][HeroX] > 0)
        {
            for (int x = 0; x < DungeonWidth; x++)
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
