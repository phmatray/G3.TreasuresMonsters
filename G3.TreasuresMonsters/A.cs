using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters;

enum CellType
{
    Empty,
    Treasure,
    Monster
}

class Cell
{
    public CellType Type { get; set; }
    public int Value { get; set; } // For Treasure and Monster
}

class Hero
{
    public int Health { get; set; } = 100;
    public int Score { get; set; } = 0;
    public int X { get; set; }
    public int Y { get; set; }
    public int Hints { get; set; } = 0;
}

class Dungeon
{
    public int Width { get; }
    public int Height { get; }
    public Cell[,] Grid { get; private set; }

    // Ajout des tableaux pour les monstres et les trésors
    public int[][] Monsters { get; private set; }
    public int[][] Treasures { get; private set; }

    public Dungeon(int width, int height)
    {
        Width = width;
        Height = height;
        GenerateLevel();
        Algorithms.DC.SortLevel(Monsters, Treasures); // Trier le niveau après la génération
        BuildGridFromArrays(); // Reconstruire la grille à partir des tableaux triés
    }

    private void GenerateLevel()
    {
        // Initialiser les tableaux des monstres et des trésors
        Monsters = new int[Height][];
        Treasures = new int[Height][];
        for (int i = 0; i < Height; i++)
        {
            Monsters[i] = new int[Width];
            Treasures[i] = new int[Width];
        }

        Algorithms.GT.GenerateMonstersAndTreasures(Monsters, Treasures);

        BuildGridFromArrays();
    }

    // Méthode pour construire la grille à partir des tableaux de monstres et trésors
    private void BuildGridFromArrays()
    {
        Grid = new Cell[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Monsters[y][x] > 0)
                {
                    Grid[y, x] = new Cell { Type = CellType.Monster, Value = Monsters[y][x] };
                }
                else if (Treasures[y][x] > 0)
                {
                    Grid[y, x] = new Cell { Type = CellType.Treasure, Value = Treasures[y][x] };
                }
                else
                {
                    Grid[y, x] = new Cell { Type = CellType.Empty, Value = 0 };
                }
            }
        }
    }
}

class Game
{
    private Hero hero;
    private Dungeon dungeon;
    private int level = 1;
    private int scoreToBeat;

    public Game()
    {
        hero = new Hero();
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        int width = 7;
        int height = 5;
        dungeon = new Dungeon(width, height);
        hero.X = width / 2;
        hero.Y = 0;
        hero.Health = Math.Min(100, hero.Health + 50);

        // Créer l'état du jeu pour les algorithmes
        State state = new State
        {
            Monsters = dungeon.Monsters,
            Treasures = dungeon.Treasures,
            HeroX = hero.X,
            HeroY = hero.Y,
            HeroHealth = hero.Health
        };

        scoreToBeat = Algorithms.GS.GreedySolution(state);
        Console.WriteLine($"\n--- Niveau {level} ---");
        Console.WriteLine($"Score à battre : {scoreToBeat}");
        PlayLevel();
    }

    private void PlayLevel()
    {
        while (true)
        {
            DisplayDungeon();
            if (hero.Health <= 0)
            {
                Console.WriteLine("Vous êtes mort. Fin du jeu !");
                Environment.Exit(0);
            }

            Console.Write("Déplacez-vous (Z/Q/S/D), H pour indice, Q pour quitter : ");
            var input = Console.ReadKey().Key;
            Console.WriteLine();

            if (input == ConsoleKey.Q)
            {
                Console.WriteLine("Merci d'avoir joué !");
                Environment.Exit(0);
            }
            else if (input == ConsoleKey.H)
            {
                if (hero.Hints > 0)
                {
                    hero.Hints--;
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
                if (hero.Y == dungeon.Height)
                {
                    EndLevel();
                    level++;
                    StartNewLevel();
                    break;
                }
            }
        }
    }

    private void ShowPerfectPath()
    {
        Console.WriteLine("Calcul de la solution parfaite...");
        State state = new State
        {
            Monsters = dungeon.Monsters,
            Treasures = dungeon.Treasures,
            HeroX = hero.X,
            HeroY = hero.Y,
            HeroHealth = hero.Health
        };
        var path = Algorithms.DP.PerfectSolution(state);
        Console.WriteLine($"Chemin parfait : {path}");
    }

    private void HandleMovement(ConsoleKey key)
    {
        int newX = hero.X;
        int newY = hero.Y;

        switch (key)
        {
            case ConsoleKey.Z:
                Console.WriteLine("Impossible de remonter.");
                return;
            case ConsoleKey.S:
                newY += 1;
                break;
            case ConsoleKey.Q:
                newX -= 1;
                break;
            case ConsoleKey.D:
                newX += 1;
                break;
            default:
                Console.WriteLine("Entrée invalide.");
                return;
        }

        if (newX < 0 || newX >= dungeon.Width || newY < 0 || newY > dungeon.Height)
        {
            Console.WriteLine("Vous ne pouvez pas vous déplacer là.");
            return;
        }

        if (newY < dungeon.Height)
        {
            Cell cell = dungeon.Grid[newY, newX];

            switch (cell.Type)
            {
                case CellType.Empty:
                    break;
                case CellType.Monster:
                    hero.Health -= cell.Value;
                    Console.WriteLine($"Vous avez rencontré un monstre ! Vous perdez {cell.Value} points de vie.");
                    // Retirer le monstre de la grille et du tableau des monstres
                    cell.Type = CellType.Empty;
                    cell.Value = 0;
                    dungeon.Monsters[newY][newX] = 0;
                    break;
                case CellType.Treasure:
                    hero.Score += cell.Value;
                    Console.WriteLine($"Vous avez trouvé un trésor ! Vous gagnez {cell.Value} points.");
                    // Retirer le trésor de la grille et du tableau des trésors
                    cell.Type = CellType.Empty;
                    cell.Value = 0;
                    dungeon.Treasures[newY][newX] = 0;
                    break;
            }
        }

        hero.X = newX;
        hero.Y = newY;
    }

    private void EndLevel()
    {
        Console.WriteLine("\nNiveau terminé !");
        Console.WriteLine($"Votre score : {hero.Score}");

        if (hero.Score > scoreToBeat)
        {
            hero.Hints++;
            Console.WriteLine("Vous avez battu le score ! Vous gagnez un indice pour les niveaux suivants.");
        }
        else
        {
            Console.WriteLine("Vous n'avez pas battu le score.");
        }
    }

    private void DisplayDungeon()
    {
        Console.WriteLine($"\nVie : {hero.Health} / 100 | Score : {hero.Score} | Indices : {hero.Hints}\n");

        for (int y = 0; y < dungeon.Height; y++)
        {
            for (int x = 0; x < dungeon.Width; x++)
            {
                if (hero.X == x && hero.Y == y)
                {
                    Console.Write("H ");
                }
                else
                {
                    switch (dungeon.Grid[y, x].Type)
                    {
                        case CellType.Empty:
                            Console.Write(". ");
                            break;
                        case CellType.Monster:
                            Console.Write("M ");
                            break;
                        case CellType.Treasure:
                            Console.Write("T ");
                            break;
                    }
                }
            }

            Console.WriteLine();
        }

        if (hero.Y == dungeon.Height)
        {
            Console.WriteLine("Vous êtes en bas du donjon. Appuyez sur 'S' pour passer au niveau suivant.");
        }
    }
}