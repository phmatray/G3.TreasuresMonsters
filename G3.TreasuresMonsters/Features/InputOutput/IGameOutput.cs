namespace G3.TreasuresMonsters.Features.InputOutput;

public interface IGameOutput
{
    void SetState(State state);
    void DisplayScreen(bool clearContextMessages = true);
    void DisplayMessage(LanguageKey key, params object[] args);
    void AddStatusMessage(LanguageKey key, params object[] args);
    void AddContextMessage(LanguageKey key, params object[] args);
}