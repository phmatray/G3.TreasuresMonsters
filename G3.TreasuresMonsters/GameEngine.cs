using G3.TreasuresMonsters.Logic;
using G3.TreasuresMonsters.Models;
using G3.TreasuresMonsters.Services;

namespace G3.TreasuresMonsters;

public class GameEngine(
    IGameInput input,
    IGameOutput output)
{
    private const int Width = 7;
    private const int Height = 11;
    
    private readonly Hero _hero = new();
    private Dungeon _dungeon;
    private int _level = 1;
    private int _scoreToBeat;
    private MovementDirection _movementDirection = MovementDirection.None;

    public async Task StartNewGame()
    {
        await StartNewLevel();
    }

    private async Task StartNewLevel()
    {
        InitializeDungeon();
        InitializeHero();
        _scoreToBeat = CalculateScoreToBeat();
        output.ClearScreen();
        output.DisplayMessage($"\n--- Niveau {_level} ---");
        output.DisplayMessage($"Score à battre : {_scoreToBeat}\n");
        await PlayLevel();
    }

    private void InitializeDungeon()
    {
        _dungeon = new Dungeon(Width, Height);
        _dungeon.Grid[0, Width / 2].Type = CellType.Empty;
        _dungeon.Grid[0, Width / 2].Value = 0;
    }

    private void InitializeHero()
    {
        _hero.X = _dungeon.Width / 2;
        _hero.Y = 0;
        _hero.Health = Math.Min(100, _hero.Health + 50);
        _dungeon.Monsters[_hero.Y][_hero.X] = 0;
        _dungeon.Treasures[_hero.Y][_hero.X] = 0;
    }

    private int CalculateScoreToBeat()
    {
        State state = new State(_hero.HeroPos, _hero.Health, _hero.Score, _dungeon.Monsters, _dungeon.Treasures, _hero.NbHint, _level);
        return Algorithms.GS.GreedySolution(state);
    }

    private async Task PlayLevel()
    {
        while (true)
        {
            output.DisplayDungeon(_dungeon, _hero, _level, _scoreToBeat);
            if (IsHeroDead())
            {
                return;
            }

            Console.Write("Déplacez-vous (↑/←/↓/→), H pour indice, Q pour quitter : ");
            var input1 = await input.GetInputAsync();
            output.DisplayMessage("");

            if (!await HandleInput(input1))
            {
                break;
            }
        }
    }

    private bool IsHeroDead()
    {
        if (_hero.Health > 0) return false;
        output.DisplayMessage("Vous êtes mort. Fin du jeu !");
        Environment.Exit(0);
        return true;
    }

    private async Task<bool> HandleInput(ConsoleKey inputKey)
    {
        switch (inputKey)
        {
            case ConsoleKey.Q:
                output.DisplayMessage("Merci d'avoir joué !");
                Environment.Exit(0);
                return false;
            case ConsoleKey.H:
                ShowHint();
                return true;
            default:
                HandleMovement(inputKey);
                if (_hero.Y >= _dungeon.Height)
                {
                    EndLevel();
                    _level++;
                    await StartNewLevel();
                    return false;
                }
                return true;
        }
    }

    private void ShowHint()
    {
        if (_hero.NbHint <= 0)
        {
            output.DisplayMessage("Aucun indice disponible.");
            return;
        }

        _hero.NbHint--;
        output.DisplayMessage("Calcul de la solution parfaite...");
        State state = new State(_hero.HeroPos, _hero.Health, _hero.Score, _dungeon.Monsters, _dungeon.Treasures, _hero.NbHint, _level);
        var path = Algorithms.DP.PerfectSolution(state);
        output.DisplayMessage($"Chemin parfait : {path}\n");
    }

    private void HandleMovement(ConsoleKey key)
    {
        int newX = _hero.X;
        int newY = _hero.Y;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                output.DisplayMessage("Impossible de remonter.");
                return;
            case ConsoleKey.DownArrow:
                newY += 1;
                _movementDirection = MovementDirection.None;
                break;
            case ConsoleKey.LeftArrow:
                if (_movementDirection == MovementDirection.Right)
                {
                    output.DisplayMessage("Vous ne pouvez pas changer de direction vers la gauche.");
                    return;
                }
                newX -= 1;
                _movementDirection = MovementDirection.Left;
                break;
            case ConsoleKey.RightArrow:
                if (_movementDirection == MovementDirection.Left)
                {
                    output.DisplayMessage("Vous ne pouvez pas changer de direction vers la droite.");
                    return;
                }
                newX += 1;
                _movementDirection = MovementDirection.Right;
                break;
            default:
                output.DisplayMessage("Entrée invalide.");
                return;
        }

        if (!IsValidMove(newX, newY))
        {
            return;
        }

        if (newY == _dungeon.Height)
        {
            _hero.Y = newY;
            return;
        }

        UpdateHeroPosition(newX, newY);
    }

    private bool IsValidMove(int newX, int newY)
    {
        if (newX < 0 || newX >= _dungeon.Width || newY < 0 || newY > _dungeon.Height)
        {
            output.DisplayMessage("Vous ne pouvez pas vous déplacer là.");
            return false;
        }
        return true;
    }

    private void UpdateHeroPosition(int newX, int newY)
    {
        Cell cell = _dungeon.Grid[newY, newX];

        switch (cell.Type)
        {
            case CellType.Empty:
                break;
            case CellType.Monster:
                _hero.Health -= cell.Value;
                output.DisplayMessage($"Vous avez rencontré un monstre ! Vous perdez {cell.Value} points de vie.");
                ClearCell(cell, newY, newX);
                break;
            case CellType.Treasure:
                _hero.Score += cell.Value;
                output.DisplayMessage($"Vous avez trouvé un trésor ! Vous gagnez {cell.Value} points.");
                ClearCell(cell, newY, newX);
                break;
        }

        _hero.X = newX;
        _hero.Y = newY;
    }

    private void ClearCell(Cell cell, int y, int x)
    {
        cell.Type = CellType.Empty;
        cell.Value = 0;
        _dungeon.Monsters[y][x] = 0;
        _dungeon.Treasures[y][x] = 0;
    }

    private void EndLevel()
    {
        output.DisplayMessage("\nNiveau terminé !");
        output.DisplayMessage($"Votre score : {_hero.Score}\n");

        if (_hero.Score > _scoreToBeat)
        {
            _hero.NbHint++;
            output.DisplayMessage("Vous avez battu le score ! Vous gagnez un indice pour les niveaux suivants.\n");
        }
        else
        {
            output.DisplayMessage("Vous n'avez pas battu le score.\n");
        }
    }
}