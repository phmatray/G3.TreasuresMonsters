using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Models;

namespace G3.TreasuresMonsters.Features.InputOutput;

public interface IGameOutput
{
    void ClearScreen();
    void DisplayBlankLine();
    void DisplayMessage(string message);
    void DisplayMessage(LanguageKey key, params object[] args);
    void DisplayDungeon(Dungeon dungeon, Hero hero, int level, int scoreToBeat);
}