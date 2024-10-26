namespace G3.TreasuresMonsters.Features.InputOutput;

public interface IGameInput
{
    Task<ConsoleKey> GetInputAsync();
}