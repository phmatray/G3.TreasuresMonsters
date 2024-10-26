namespace G3.TreasuresMonsters.Features.InputOutput;

public class ConsoleGameInput : IGameInput
{
    public async Task<ConsoleKey> GetInputAsync()
    {
        return await Task.Run(() => Console.ReadKey().Key);
    }
}