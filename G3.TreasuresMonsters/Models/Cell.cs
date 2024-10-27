namespace G3.TreasuresMonsters.Models;

public class Cell
{
    public CellType Type { get; set; }
    public int Value { get; set; } // For Treasure and Monster
    
    public static Cell CreateEmptyCell()
    {
        return new Cell
        {
            Type = CellType.Empty,
            Value = 0
        };
    }
    
    public static Cell CreateMonsterCell(int value)
    {
        return new Cell
        {
            Type = CellType.Monster,
            Value = value
        };
    }
    
    public static Cell CreateTreasureCell(int value)
    {
        return new Cell
        {
            Type = CellType.Treasure,
            Value = value
        };
    }
}