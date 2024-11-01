using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("G3.TreasuresMonsters.Tests")]

new ServiceCollection()
    .AddSingleton<IGameInput, ConsoleGameInput>()
    .AddSingleton<IGameOutput, ConsoleGameOutput>()
    .AddSingleton<ILanguageService, LanguageService>()
    .AddSingleton<GameEngine>()
    .BuildServiceProvider()
    .GetRequiredService<GameEngine>()
    .StartNewGame();
