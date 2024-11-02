namespace G3.TreasuresMonsters.Features.InputOutput;

public class ConsoleGameOutput(ILanguageService language)
    : IGameOutput
{
    private readonly List<string> _statusMessages = [];
    private readonly List<string> _contextMessages = [];
    private readonly List<string> _dungeonRows = [];
    private State _currentState = null!;
    
    public void SetState(State state)
    {
        _currentState = state;
        
        // Clear messages for the next frame
        _statusMessages.Clear();
        _dungeonRows.Clear();
        
        AddStatusMessage(LanguageKey.Level, state.NbLevel);
        AddStatusMessage(LanguageKey.ScoreToBeat, state.DungeonScoreToBeat);
        AddStatusMessage(LanguageKey.HeroStatus, state.HeroHealth, state.HeroScore, state.NbHint);

        BuildDungeonRows(state);
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
    
    private void BuildDungeonRows(State state)
    {
        // Build the top row of the dungeon
        string topWall = BuildDungeonTopWall(state);
        _dungeonRows.Add(topWall);

        // Build the center of the dungeon
        for (int y = 0; y < _currentState.DungeonHeight; y++)
        {
            var middleRow = BuildDungeonMiddleRow(state, y);
            _dungeonRows.Add(middleRow);
        }

        // Build the bottom row of the dungeon
        string bottomWall = BuildDungeonBottomWall(state);
        _dungeonRows.Add(bottomWall);
    }

    private static string BuildDungeonTopWall(State state)
    {
        return Constants.WallCornerTopLeft +
               new string(Constants.WallTop[0], state.DungeonWidth * 5 + 1) +
               Constants.WallCornerTopRight;
    }

    private string BuildDungeonMiddleRow(State state, int rowIndex)
    {
        StringBuilder sb = new();
        sb.Append($"{Constants.WallLeft} "); // Left wall

        int rowValue = 0;
        
        for (int x = 0; x < state.DungeonWidth; x++)
        {
            var monsterStrength = _currentState.Monsters[rowIndex][x];
            var treasureValue = _currentState.Treasures[rowIndex][x];

            if (_currentState.HeroX == x && _currentState.HeroY == rowIndex)
            {
                sb.Append($"{Constants.GetHeroEmoji(_currentState.HeroIsAlive)}   "); // Hero emoji with consistent spacing
            }
            else if (monsterStrength > 0)
            {
                sb.Append(Constants.GetMonsterEmoji(monsterStrength)); // Monster emoji
                sb.Append($"{monsterStrength:D2} "); // Monster strength
                rowValue -= monsterStrength;
            }
            else if (treasureValue > 0)
            {
                sb.Append(Constants.GetTreasureEmoji(treasureValue)); // Treasure emoji
                sb.Append($"{treasureValue:D2} "); // Treasure value
                rowValue += treasureValue;
            }
            else
            {
                sb.Append($"{Constants.EmptyCell}    ");
            }
        }

        sb.Append(Constants.WallRight); // Right wall
        sb.Append($" {rowValue:D3}"); // Row value

        return sb.ToString();
    }
    
    private static string BuildDungeonBottomWall(State state)
    {
        return Constants.WallCornerBottomLeft +
               new string(Constants.WallBottom[0], state.DungeonWidth * 5 + 1) +
               Constants.WallCornerBottomRight;
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