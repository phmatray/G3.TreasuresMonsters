namespace G3.TreasuresMonsters.Features.InputOutput;

public class ConsoleGameOutput(ILanguageService language)
    : IGameOutput
{
    private readonly List<string> _statusMessages = [];
    private readonly List<string> _contextMessages = [];
    private readonly List<string> _dungeonRows = [];
    private State? _currentState;
    
    public void SetState(State state)
    {
        _currentState = state;
        
        // Clear messages for the next frame
        _statusMessages.Clear();
        _dungeonRows.Clear();
        
        AddStatusMessage(LanguageKey.Level, state.NbLevel);
        AddStatusMessage(LanguageKey.ScoreToBeat, state.DungeonScoreToBeat);
        AddStatusMessage(LanguageKey.HeroStatus, state.HeroHealth, state.HeroScore, state.NbHint);

        BuildDungeonRows();
    }

    public void DisplayScreen(bool clearContextMessages = true)
    {
        Console.Clear(); // Clear the console to update the screen

        // Display status messages
        foreach (var message in _statusMessages)
        {
            Console.WriteLine(message);
        }

        // Display a blank line to separate sections
        Console.WriteLine();

        // Display the dungeon
        foreach (var row in _dungeonRows)
        {
            Console.WriteLine(row);
        }

        // Display a blank line to separate sections
        Console.WriteLine();
        
        // Display context messages
        foreach (var message in _contextMessages)
        {
            Console.WriteLine(message);
        }
        
        // Clear context messages if requested
        if (clearContextMessages)
        {
            _contextMessages.Clear();
        }
    }
    
    private void BuildDungeonRows()
    {
        if (_currentState == null)
        {
            return;
        }
        
        // Build the top row of the dungeon
        string topWall = 
            Constants.WallCornerTopLeft +
            new string(Constants.WallTop[0], Constants.DungeonWidth * 5 + 1) +
            Constants.WallCornerTopRight;
        
        _dungeonRows.Add(topWall);

        // Build the center of the dungeon
        for (int y = 0; y < Constants.DungeonHeight; y++)
        {
            int rowValue = 0;
            StringBuilder row = new();
            row.Append($"{Constants.WallLeft} "); // Left wall

            for (int x = 0; x < Constants.DungeonWidth; x++)
            {
                var monsterStrength = _currentState.Monsters[y][x];
                var treasureValue = _currentState.Treasures[y][x];

                if (_currentState.HeroX == x && _currentState.HeroY == y)
                {
                    row.Append($"{Constants.GetHeroEmoji(_currentState.HeroIsAlive)}   "); // Hero emoji with consistent spacing
                }
                else if (monsterStrength > 0)
                {
                    row.Append(Constants.GetMonsterEmoji(monsterStrength)); // Monster emoji
                    row.Append($"{monsterStrength:D2} "); // Monster strength
                    rowValue -= monsterStrength;
                }
                else if (treasureValue > 0)
                {
                    row.Append(Constants.GetTreasureEmoji(treasureValue)); // Treasure emoji
                    row.Append($"{treasureValue:D2} "); // Treasure value
                    rowValue += treasureValue;
                }
                else
                {
                    row.Append($"{Constants.EmptyCell}    ");
                }
            }

            row.Append(Constants.WallRight); // Right wall
            row.Append($" {rowValue:D3}"); // Row value
            _dungeonRows.Add(row.ToString());
        }

        // Build the bottom row of the dungeon
        string bottomWall = 
            Constants.WallCornerBottomLeft +
            new string(Constants.WallBottom[0], Constants.DungeonWidth * 5 + 1) +
            Constants.WallCornerBottomRight;

        _dungeonRows.Add(bottomWall);
    }

    public void AddStatusMessage(LanguageKey key, params object[] args)
    {
        var message = GetMessage(key, args);
        _statusMessages.Add(message);
    }
    
    public void AddContextMessage(LanguageKey key, params object[] args)
    {
        var message = GetMessage(key, args);
        _contextMessages.Add(message);
    }

    public void DisplayMessage(LanguageKey key, params object?[] args)
    {
        var message = GetMessage(key, args);
        Console.WriteLine(message);
    }
    
    private string GetMessage(LanguageKey key, params object?[] args)
    {
        var format = language.GetString(key);
        return string.Format(format, args);
    }
}