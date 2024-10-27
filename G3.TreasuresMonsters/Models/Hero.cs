namespace G3.TreasuresMonsters.Models;

public class Hero
{
    /// <summary>
    /// Current health of the hero, between 0 and 100
    /// </summary>
    public int Health { get; private set; } = 100;
    
    public bool IsDead => Health <= 0;
    
    /// <summary>
    /// Current score of the hero
    /// </summary>
    public int Score { get; private set; }
    
    public MovementConstraint MoveConstraint { get; private set; } = MovementConstraint.None;
    
    public int X { get; private set; }
    
    public int Y { get; private set; }
    
    /// <summary>
    /// Number of hints available to the player
    /// </summary>
    public int NbHint { get; private set; }

    public void SetPosition(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }
    
    public void MoveLeft()
    {
        X--;
        MoveConstraint = MovementConstraint.Right;
    }
    
    public void MoveRight()
    {
        X++;
        MoveConstraint = MovementConstraint.Left;
    }
    
    public void MoveDown()
    {
        Y++;
        MoveConstraint = MovementConstraint.None;
    }

    public void DecreaseHealth(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value must be positive");

        if (Health - value >= 0)
            Health -= value;
        else
            Health = 0;
    }

    public void IncreaseScore(int value)
    {
        if (value < 0)
            throw new ArgumentException("Value must be positive");

        Score += value;
    }

    public void AddHint()
    {
        NbHint++;
    }

    public void DecreaseHint()
    {
        if (NbHint > 0)
            NbHint--;
    }

    public void GainHealth(int value)
    {
        Health = Math.Min(100, Health + value);
    }
}