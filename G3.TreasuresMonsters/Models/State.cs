namespace G3.TreasuresMonsters.Models;

public class State
{
    public int[][] Monsters { get; set; }   // Tableau 2D des monstres (force), 0 si pas de monstre
    public int[][] Treasures { get; set; }  // Tableau 2D des trésors (valeur), 0 si pas de trésor
    public int HeroX { get; set; }          // Position X du héros
    public int HeroY { get; set; }          // Position Y du héros
    public int HeroHealth { get; set; }     // Santé du héros
}