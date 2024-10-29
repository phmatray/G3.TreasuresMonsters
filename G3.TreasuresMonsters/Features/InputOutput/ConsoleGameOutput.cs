using System.Text;
using G3.TreasuresMonsters.Features.Engine;
using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Models;

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
        AddStatusMessage(LanguageKey.ScoreToBeat, state.Dungeon.ScoreToBeat);
        AddStatusMessage(LanguageKey.HeroStatus, state.Hero.Health, state.Hero.Score, state.Hero.NbHint);

        BuildDungeonRows();
    }

    public void DisplayScreen()
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
    }
    
    private void BuildDungeonRows()
    {
        if (_currentState == null)
        {
            return;
        }
        
        // Build the top row of the dungeon
        string topWall = "╔" + new string('═', (_currentState.Dungeon.Width * 5) + 1) + "╗";
        _dungeonRows.Add(topWall);

        // Build the center of the dungeon
        for (int y = 0; y < _currentState.Dungeon.Height; y++)
        {
            int rowValue = 0;
            StringBuilder row = new();
            row.Append("║ "); // Left wall
            
            for (int x = 0; x < _currentState.Dungeon.Width; x++)
            {
                if (_currentState.Hero.X == x && _currentState.Hero.Y == y)
                {
                    row.Append("🦄   "); // Hero emoji with consistent spacing
                }
                else
                {
                    Cell cell = _currentState.Dungeon.Grid[y, x];
                    switch (cell.Type)
                    {
                        case CellType.Empty:
                            row.Append(".    ");
                            break;
                        case CellType.Monster:
                            row.Append($"👹{cell.Value:D2} "); // Monster emoji with strength
                            rowValue -= cell.Value;
                            break;
                        case CellType.Treasure:
                            row.Append($"💰{cell.Value:D2} "); // Treasure emoji with value
                            rowValue += cell.Value;
                            break;
                    }
                }
            }
            
            row.Append('║'); // Right wall
            row.Append($" {rowValue:D3}"); // Row value
            _dungeonRows.Add(row.ToString());
        }

        // Build the bottom row of the dungeon
        string bottomWall = "╚" + new string('═', (_currentState.Dungeon.Width * 5) + 1) + "╝";
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
    
    public void ClearContextMessages()
    {
        _contextMessages.Clear();
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