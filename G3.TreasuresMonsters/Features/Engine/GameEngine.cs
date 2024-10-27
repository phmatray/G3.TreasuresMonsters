using G3.TreasuresMonsters.Features.I18n;
using G3.TreasuresMonsters.Features.InputOutput;
using G3.TreasuresMonsters.Features.Logic;
using G3.TreasuresMonsters.Models;
// ReSharper disable InconsistentNaming

namespace G3.TreasuresMonsters.Features.Engine;

public class GameEngine(
    IGameInput _input,
    IGameOutput _output)
{
    private readonly State _state = new();
    private MovementDirection _movementDirection = MovementDirection.None;
    
    public async Task StartNewGame()
    {
        await StartNewLevel();
    }

    private async Task StartNewLevel()
    {
        _state.Dungeon = new Dungeon();
        _state.Hero.SetPosition(_state.Dungeon.Width / 2, 0);
        _state.Hero.GainHealth(50);
        _state.Dungeon.Monsters[_state.Hero.Y][_state.Hero.X] = 0;
        _state.Dungeon.Treasures[_state.Hero.Y][_state.Hero.X] = 0;
        _state.Dungeon.ScoreToBeat = CalculateScoreToBeat();
        _output.ClearScreen();
        _output.DisplayBlankLine();
        _output.DisplayMessage(LanguageKey.Level, _state.NbLevel);
        _output.DisplayMessage(LanguageKey.ScoreToBeat, _state.Dungeon.ScoreToBeat);
        await PlayLevel();
    }

    private int CalculateScoreToBeat()
    {
        return Algorithms.GS.GreedySolution(_state);
    }

    private async Task PlayLevel()
    {
        while (true)
        {
            _output.DisplayDungeon(_state.Dungeon, _state.Hero, _state.NbLevel, _state.Dungeon.ScoreToBeat);
            if (_state.Hero.IsDead)
            {
                _output.DisplayMessage(LanguageKey.GameOver);
                Environment.Exit(0);
            }

            _output.DisplayMessage(LanguageKey.MovePrompt);
            var inputKey = await _input.GetInputAsync();
            _output.DisplayMessage("");

            if (!await HandleInput(inputKey))
            {
                break;
            }
        }
    }

    private async Task<bool> HandleInput(ConsoleKey inputKey)
    {
        switch (inputKey)
        {
            case ConsoleKey.Q:
                _output.DisplayMessage(LanguageKey.ThanksForPlaying);
                Environment.Exit(0);
                return false;
            case ConsoleKey.H:
                ShowHint();
                return true;
            default:
                HandleMovement(inputKey);
                if (_state.Hero.Y >= _state.Dungeon.Height)
                {
                    EndLevel();
                    _state.NbLevel++;
                    await StartNewLevel();
                    return false;
                }
                return true;
        }
    }

    private void ShowHint()
    {
        if (_state.Hero.NbHint <= 0)
        {
            _output.DisplayMessage(LanguageKey.NoHintAvailable);
        }
        else
        {
            _state.Hero.DecreaseHint();
            _output.DisplayMessage(LanguageKey.CalculatingPerfectSolution);
            var path = Algorithms.DP.PerfectSolution(_state);
            _output.DisplayMessage(LanguageKey.PerfectPath, path);
            _output.DisplayBlankLine();
        }
    }

    private void HandleMovement(ConsoleKey key)
    {
        int newX = _state.Hero.X;
        int newY = _state.Hero.Y;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                _output.DisplayMessage(LanguageKey.CannotMoveUp);
                return;
            case ConsoleKey.DownArrow:
                newY += 1;
                _movementDirection = MovementDirection.None;
                break;
            case ConsoleKey.LeftArrow:
                if (_movementDirection == MovementDirection.Right)
                {
                    _output.DisplayMessage(LanguageKey.CannotMoveLeft);
                    return;
                }
                newX -= 1;
                _movementDirection = MovementDirection.Left;
                break;
            case ConsoleKey.RightArrow:
                if (_movementDirection == MovementDirection.Left)
                {
                    _output.DisplayMessage(LanguageKey.CannotMoveRight);
                    return;
                }
                newX += 1;
                _movementDirection = MovementDirection.Right;
                break;
            default:
                _output.DisplayMessage(LanguageKey.InvalidInput);
                return;
        }

        if (!IsValidMove(newX, newY))
        {
            return;
        }

        if (newY == _state.Dungeon.Height)
        {
            _state.Hero.SetPosition(newX, newY);
            return;
        }

        UpdateHeroPosition(newX, newY);
    }

    private bool IsValidMove(int newX, int newY)
    {
        if (newX < 0 || newX >= _state.Dungeon.Width || newY < 0 || newY > _state.Dungeon.Height)
        {
            _output.DisplayMessage(LanguageKey.CannotMoveThere);
            return false;
        }
        return true;
    }

    private void UpdateHeroPosition(int newX, int newY)
    {
        Cell cell = _state.Dungeon.Grid[newY, newX];

        switch (cell.Type)
        {
            case CellType.Empty:
                break;
            case CellType.Monster:
                _state.Hero.DecreaseHealth(cell.Value);
                _output.DisplayMessage(LanguageKey.MonsterEncounter, cell.Value);
                ClearCell(cell, newY, newX);
                break;
            case CellType.Treasure:
                _state.Hero.IncreaseScore(cell.Value);
                _output.DisplayMessage(LanguageKey.TreasureFound, cell.Value);
                ClearCell(cell, newY, newX);
                break;
        }

        _state.Hero.SetPosition(newX, newY);
    }

    private void ClearCell(Cell cell, int y, int x)
    {
        cell.Type = CellType.Empty;
        cell.Value = 0;
        _state.Dungeon.Monsters[y][x] = 0;
        _state.Dungeon.Treasures[y][x] = 0;
    }

    private void EndLevel()
    {
        _output.DisplayBlankLine();
        _output.DisplayMessage(LanguageKey.LevelCompleted);
        _output.DisplayMessage(LanguageKey.YourScore, _state.Hero.Score);
        _output.DisplayBlankLine();

        if (_state.Hero.Score > _state.Dungeon.ScoreToBeat)
        {
            _state.Hero.AddHint();
            _output.DisplayMessage(LanguageKey.BeatScore);
        }
        else
        {
            _output.DisplayMessage(LanguageKey.DidNotBeatScore);
        }

        _output.DisplayBlankLine();
    }
}