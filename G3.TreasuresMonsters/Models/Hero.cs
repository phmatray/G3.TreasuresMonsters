namespace G3.TreasuresMonsters.Models;

public class Hero
{
    public int Health { get; set; } = 100;
    public int Score { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int NbHint { get; set; }
    
    public int[] HeroPos => [X, Y];
}