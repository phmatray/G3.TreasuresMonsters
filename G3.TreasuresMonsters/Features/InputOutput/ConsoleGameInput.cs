namespace G3.TreasuresMonsters.Features.InputOutput;

public class ConsoleGameInput : IGameInput
{
    public ConsoleKey GetInput()
    {
        return Console.ReadKey().Key;
    }
}