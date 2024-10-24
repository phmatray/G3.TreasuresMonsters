using G3.TreasuresMonsters.Logic;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters;

public class Game
{
    private readonly Hero _hero;
    private Dungeon _dungeon;
    private int _level = 1;
    private int _scoreToBeat;

    public Game()
    {
        _hero = new Hero();
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        int width = 7;
        int height = 11;
        _dungeon = new Dungeon(width, height);
        _hero.X = width / 2;
        _hero.Y = 0;
        _hero.Health = Math.Min(100, _hero.Health + 50);

        // S'assurer que la position de départ du héros est vide
        _dungeon.Grid[_hero.Y, _hero.X].Type = CellType.Empty;
        _dungeon.Grid[_hero.Y, _hero.X].Value = 0;
        _dungeon.Monsters[_hero.Y][_hero.X] = 0;
        _dungeon.Treasures[_hero.Y][_hero.X] = 0;

        // Créer l'état du jeu pour les algorithmes
        State state = new State(_hero.HeroPos, _hero.Health, _hero.Score, _dungeon.Monsters, _dungeon.Treasures, _hero.NbHint, _level);
        _scoreToBeat = Algorithms.GS.GreedySolution(state);
        Console.Clear();
        Console.WriteLine($"\n--- Niveau {_level} ---");
        Console.WriteLine($"Score à battre : {_scoreToBeat}\n");
        PlayLevel();
    }

    private void PlayLevel()
    {
        while (true)
        {
            DisplayDungeon();
            if (_hero.Health <= 0)
            {
                Console.WriteLine("Vous êtes mort. Fin du jeu !");
                Environment.Exit(0);
            }

            Console.Write("Déplacez-vous (↑/←/↓/→), H pour indice, Q pour quitter : ");
            var input = Console.ReadKey().Key;
            Console.WriteLine();

            if (input == ConsoleKey.Q)
            {
                Console.WriteLine("Merci d'avoir joué !");
                Environment.Exit(0);
            }
            else if (input == ConsoleKey.H)
            {
                if (_hero.NbHint > 0)
                {
                    _hero.NbHint--;
                    ShowPerfectPath();
                }
                else
                {
                    Console.WriteLine("Aucun indice disponible.");
                }
            }
            else
            {
                HandleMovement(input);
                if (_hero.Y >= _dungeon.Height)
                {
                    EndLevel();
                    _level++;
                    StartNewLevel();
                    break;
                }
            }
        }
    }

    private void ShowPerfectPath()
    {
        Console.WriteLine("Calcul de la solution parfaite...");
        State state = new State(_hero.HeroPos, _hero.Health, _hero.Score, _dungeon.Monsters, _dungeon.Treasures, _hero.NbHint, _level);
        var path = Algorithms.DP.PerfectSolution(state);
        Console.WriteLine($"Chemin parfait : {path}\n");
    }

    private void HandleMovement(ConsoleKey key)
    {
        int newX = _hero.X;
        int newY = _hero.Y;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                Console.WriteLine("Impossible de remonter.");
                return;
            case ConsoleKey.DownArrow:
                newY += 1;
                break;
            case ConsoleKey.LeftArrow:
                newX -= 1;
                break;
            case ConsoleKey.RightArrow:
                newX += 1;
                break;
            default:
                Console.WriteLine("Entrée invalide.");
                return;
        }

        if (newX < 0 || newX >= _dungeon.Width || newY < 0 || newY > _dungeon.Height)
        {
            Console.WriteLine("Vous ne pouvez pas vous déplacer là.");
            return;
        }

        if (newY < _dungeon.Height)
        {
            Cell cell = _dungeon.Grid[newY, newX];

            switch (cell.Type)
            {
                case CellType.Empty:
                    break;
                case CellType.Monster:
                    _hero.Health -= cell.Value;
                    Console.WriteLine($"Vous avez rencontré un monstre ! Vous perdez {cell.Value} points de vie.");
                    // Retirer le monstre de la grille et du tableau des monstres
                    cell.Type = CellType.Empty;
                    cell.Value = 0;
                    _dungeon.Monsters[newY][newX] = 0;
                    break;
                case CellType.Treasure:
                    _hero.Score += cell.Value;
                    Console.WriteLine($"Vous avez trouvé un trésor ! Vous gagnez {cell.Value} points.");
                    // Retirer le trésor de la grille et du tableau des trésors
                    cell.Type = CellType.Empty;
                    cell.Value = 0;
                    _dungeon.Treasures[newY][newX] = 0;
                    break;
            }
        }

        _hero.X = newX;
        _hero.Y = newY;
    }

    private void EndLevel()
    {
        Console.WriteLine("\nNiveau terminé !");
        Console.WriteLine($"Votre score : {_hero.Score}\n");

        if (_hero.Score > _scoreToBeat)
        {
            _hero.NbHint++;
            Console.WriteLine("Vous avez battu le score ! Vous gagnez un indice pour les niveaux suivants.\n");
        }
        else
        {
            Console.WriteLine("Vous n'avez pas battu le score.\n");
        }
    }

    private void DisplayDungeon()
    {
        Console.Clear();
        Console.WriteLine($"\nNiveau : {_level}");
        Console.WriteLine($"Vie : {_hero.Health} / 100 | Score : {_hero.Score} | Indices : {_hero.NbHint}\n");

        string topWall = "╔" + new string('═', (_dungeon.Width * 5) + 1) + "╗";
        string bottomWall = "╚" + new string('═', (_dungeon.Width * 5) + 1) + "╝";
        Console.WriteLine(topWall);

        for (int y = 0; y < _dungeon.Height; y++)
        {
            Console.Write("║ "); // Left wall
            for (int x = 0; x < _dungeon.Width; x++)
            {
                if (_hero.X == x && _hero.Y == y)
                {
                    Console.Write("🦄   "); // Hero emoji with consistent spacing
                }
                else
                {
                    switch (_dungeon.Grid[y, x].Type)
                    {
                        case CellType.Empty:
                            Console.Write(".    ");
                            break;
                        case CellType.Monster:
                            Console.Write($"👹{_dungeon.Grid[y, x].Value:D2} "); // Monster emoji with strength
                            break;
                        case CellType.Treasure:
                            Console.Write($"💰{_dungeon.Grid[y, x].Value:D2} "); // Treasure emoji with value
                            break;
                    }
                }
            }
            Console.WriteLine("║"); // Right wall
        }

        Console.WriteLine(bottomWall);

        if (_hero.Y == _dungeon.Height)
        {
            Console.WriteLine("Vous êtes en bas du donjon. Appuyez sur '↓' pour passer au niveau suivant.");
        }
    }
}
