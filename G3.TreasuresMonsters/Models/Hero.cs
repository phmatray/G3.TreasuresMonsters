namespace G3.TreasuresMonsters.Models;

public class Hero
{
    public int Health { get; set; } = 100;
    public int Score { get; set; } = 0;
    public int X { get; set; }
    public int Y { get; set; }
    public int Hints { get; set; } = 0;
}