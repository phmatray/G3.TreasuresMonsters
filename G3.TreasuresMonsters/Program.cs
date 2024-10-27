using G3.TreasuresMonsters.Features.Engine;
using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Features.InputOutput;
using Microsoft.Extensions.DependencyInjection;

new ServiceCollection()
    .AddSingleton<IGameInput, ConsoleGameInput>()
    .AddSingleton<IGameOutput, ConsoleGameOutput>()
    .AddSingleton<ILanguageService, LanguageService>()
    .AddSingleton<GameEngine>()
    .BuildServiceProvider()
    .GetRequiredService<GameEngine>()
    .StartNewGame();
