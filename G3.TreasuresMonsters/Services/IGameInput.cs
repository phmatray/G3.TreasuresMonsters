namespace G3.TreasuresMonsters.Services;

public interface IGameInput
{
    Task<ConsoleKey> GetInputAsync();
}