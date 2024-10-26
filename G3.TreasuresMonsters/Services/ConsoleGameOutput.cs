using System.Text;
using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Services;

public class ConsoleGameOutput(ILanguageService language)
    : IGameOutput
{
    public void ClearScreen()
    {
        Console.Clear();
    }

    public void DisplayBlankLine()
    {
        Console.WriteLine();
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayMessage(LanguageKey key, params object?[] args)
    {
        var format = language.GetString(key);
        var message = string.Format(format, args);
        DisplayMessage(message);
    }

    public void DisplayDungeon(Dungeon dungeon, Hero hero, int level, int scoreToBeat)
    {
        ClearScreen();
        DisplayBlankLine();
        DisplayMessage(LanguageKey.LevelSummary, level);
        DisplayMessage(LanguageKey.HeroStatus, hero.Health, hero.Score, hero.NbHint);
        DisplayBlankLine();
        
        StringBuilder sb = new();
        string topWall    = "╔" + new string('═', (dungeon.Width * 5) + 1) + "╗";
        string bottomWall = "╚" + new string('═', (dungeon.Width * 5) + 1) + "╝";
        sb.AppendLine(topWall);

        for (int y = 0; y < dungeon.Height; y++)
        {
            sb.Append("║ "); // Left wall
            for (int x = 0; x < dungeon.Width; x++)
            {
                if (hero.X == x && hero.Y == y)
                {
                    sb.Append("🦄   "); // Hero emoji with consistent spacing
                }
                else
                {
                    Cell cell = dungeon.Grid[y, x];
                    switch (cell.Type)
                    {
                        case CellType.Empty:
                            sb.Append(".    ");
                            break;
                        case CellType.Monster:
                            sb.Append($"👹{cell.Value:D2} "); // Monster emoji with strength
                            break;
                        case CellType.Treasure:
                            sb.Append($"💰{cell.Value:D2} "); // Treasure emoji with value
                            break;
                    }
                }
            }
            sb.Append('║'); // Right wall
            sb.AppendLine();
        }

        sb.AppendLine(bottomWall);
        
        string[] levelLines = sb.ToString().Split('\n');
        foreach (var line in levelLines)
        {
            DisplayMessage(line);
        }

        if (hero.Y == dungeon.Height)
        {
            DisplayMessage(LanguageKey.LevelEnd);
        }
    }
}