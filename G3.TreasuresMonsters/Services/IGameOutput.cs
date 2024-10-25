using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Services;

public interface IGameOutput
{
    void ClearScreen();
    void DisplayMessage(string message);
    void DisplayDungeon(Dungeon dungeon, Hero hero, int level, int scoreToBeat);
}