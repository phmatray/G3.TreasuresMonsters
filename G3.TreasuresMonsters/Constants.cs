namespace G3.TreasuresMonsters;

public static class Constants
{
    public const int MaxHealth = 100;
    public const int DungeonWidth = 7;
    public const int DungeonHeight = 11;

    public const string HeroAlive = "🦄";
    public const string HeroDead = "💀";

    public const string MonsterA = "👾";
    public const string MonsterB = "🧟";
    public const string MonsterC = "👺";
    
    public const string TreasureA = "💰";
    public const string TreasureB = "💎";

    public const string EmptyCell = ".";
    public const string WallLeft = "║";
    public const string WallRight = "║";
    public const string WallTop = "═";
    public const string WallBottom = "═";
    public const string WallCornerTopLeft = "╔";
    public const string WallCornerTopRight = "╗";
    public const string WallCornerBottomLeft = "╚";
    public const string WallCornerBottomRight = "╝";
    
    public static string GetHeroEmoji(bool isAlive)
        => isAlive ? HeroAlive : HeroDead;
    
    public static string GetMonsterEmoji(int strength)
        => strength switch
        {
            >= 40 => MonsterC,
            >= 15 => MonsterB,
            _ => MonsterA
        };
    
    public static string GetTreasureEmoji(int value)
        => value switch
        {
            >= 80 => TreasureB,
            _ => TreasureA
        };
}