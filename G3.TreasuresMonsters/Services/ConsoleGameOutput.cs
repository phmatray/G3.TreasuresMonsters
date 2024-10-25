using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Services;

public class ConsoleGameOutput : IGameOutput
{
    public void ClearScreen()
    {
        Console.Clear();
    }
    
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayDungeon(Dungeon dungeon, Hero hero, int level, int scoreToBeat)
    {
        Console.Clear();
        Console.WriteLine($"\nNiveau : {level}");
        Console.WriteLine($"Vie : {hero.Health} / 100 | Score : {hero.Score} | Indices : {hero.NbHint}\n");

        string topWall = "╔" + new string('═', (dungeon.Width * 5) + 1) + "╗";
        string bottomWall = "╚" + new string('═', (dungeon.Width * 5) + 1) + "╝";
        Console.WriteLine(topWall);

        for (int y = 0; y < dungeon.Height; y++)
        {
            Console.Write("║ "); // Left wall
            for (int x = 0; x < dungeon.Width; x++)
            {
                if (hero.X == x && hero.Y == y)
                {
                    Console.Write("🦄   "); // Hero emoji with consistent spacing
                }
                else
                {
                    DisplayCell(dungeon.Grid[y, x]);
                }
            }
            Console.WriteLine("║"); // Right wall
        }

        Console.WriteLine(bottomWall);

        if (hero.Y == dungeon.Height)
        {
            Console.WriteLine("Vous êtes en bas du donjon. Appuyez sur '↓' pour passer au niveau suivant.");
        }
    }
    
    private void DisplayCell(Cell cell)
    {
        switch (cell.Type)
        {
            case CellType.Empty:
                Console.Write(".    ");
                break;
            case CellType.Monster:
                Console.Write($"👹{cell.Value:D2} "); // Monster emoji with strength
                break;
            case CellType.Treasure:
                Console.Write($"💰{cell.Value:D2} "); // Treasure emoji with value
                break;
        }
    }
}