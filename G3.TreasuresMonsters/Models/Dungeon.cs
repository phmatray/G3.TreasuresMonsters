using G3.TreasuresMonsters.Features.Logic;

namespace G3.TreasuresMonsters.Models;

public class Dungeon
{
    public int Width { get; } = 7;
    public int Height { get; } = 11;
    public Cell[,] Grid { get; }
    public int[][] Monsters { get; }
    public int[][] Treasures { get; }

    public Dungeon()
    {
        Monsters = new int[Height][];
        Treasures = new int[Height][];
        Grid = new Cell[Height, Width];
        
        GenerateLevel();
        // Algorithms.DC.SortLevel(Monsters, Treasures); // Trier le niveau après la génération
        BuildGridFromArrays(); // Reconstruire la grille à partir des tableaux triés
        
        Grid[0, Width / 2].Type = CellType.Empty;
        Grid[0, Width / 2].Value = 0;
    }

    private void GenerateLevel()
    {
        // Initialiser les tableaux des monstres et des trésors
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
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Monsters[y][x] > 0)
                {
                    Grid[y, x] = Cell.CreateMonsterCell(Monsters[y][x]);
                }
                else if (Treasures[y][x] > 0)
                {
                    Grid[y, x] = Cell.CreateTreasureCell(Treasures[y][x]);
                }
                else
                {
                    Grid[y, x] = Cell.CreateEmptyCell();
                }
            }
        }
    }
}