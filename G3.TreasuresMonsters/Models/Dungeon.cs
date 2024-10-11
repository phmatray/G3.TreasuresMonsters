using G3.TreasuresMonsters.Logic;

namespace G3.TreasuresMonsters.Models;

public class Dungeon
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