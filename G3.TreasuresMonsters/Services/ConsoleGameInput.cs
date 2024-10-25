namespace G3.TreasuresMonsters.Services;

public class ConsoleGameInput : IGameInput
{
    public async Task<ConsoleKey> GetInputAsync()
    {
        return await Task.Run(() => Console.ReadKey().Key);
    }
}