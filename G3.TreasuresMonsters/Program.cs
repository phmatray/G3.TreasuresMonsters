using G3.TreasuresMonsters;
using G3.TreasuresMonsters.Features.Engine;
using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Features.InputOutput;
using Microsoft.Extensions.DependencyInjection;

// Console.WriteLine("Bienvenue dans Trésors & Monstres !");
ServiceCollection services = new ServiceCollection();
services.AddSingleton<IGameInput, ConsoleGameInput>();
services.AddSingleton<IGameOutput, ConsoleGameOutput>();
services.AddSingleton<ILanguageService, LanguageService>();
services.AddSingleton<GameEngine>();
var serviceProvider = services.BuildServiceProvider();
var game = serviceProvider.GetRequiredService<GameEngine>();
await game.StartNewGame();
