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
    
    public void StartNewGame()
    {
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        _state.Dungeon = new Dungeon();
        _state.Hero.SetPosition(_state.Dungeon.Width / 2, 0);
        _state.Hero.GainHealth(50);
        _state.Dungeon.Monsters[_state.Hero.Y][_state.Hero.X] = 0;
        _state.Dungeon.Treasures[_state.Hero.Y][_state.Hero.X] = 0;
        _state.Dungeon.ScoreToBeat = Algorithms.GS.GreedySolution(_state);
        
        _output.SetState(_state);
        _output.DisplayScreen(false);
        
        PlayLevel();
    }

    private void PlayLevel()
    {
        while (true)
        {
            if (_state.Hero.IsDead)
            {
                GameOver();
            }

            _output.DisplayMessage(LanguageKey.MovePrompt);
            var inputKey = _input.GetInput();
            HandleInput(inputKey);
        }
    }

    private void HandleInput(ConsoleKey inputKey)
    {
        switch (inputKey)
        {
            case ConsoleKey.UpArrow:
                HandleMovementUp();
                break;
            case ConsoleKey.DownArrow:
                HandleMovementDown();
                break;
            case ConsoleKey.LeftArrow:
                HandleMovementLeft();
                break;
            case ConsoleKey.RightArrow:
                HandleMovementRight();
                break;
            case ConsoleKey.Q:
                QuitGame();
                break;
            case ConsoleKey.H:
                ShowHint();
                break;
            default:
                _output.AddStatusMessage(LanguageKey.InvalidInput);
                break;
        }
    }

    private void HandleMovementUp()
    {
        _output.AddContextMessage(LanguageKey.CannotMoveUp);
        _output.DisplayScreen();
    }
    
    private void HandleMovementDown()
    {
        _state.Hero.MoveDown();
            
        if (_state.Hero.Y >= _state.Dungeon.Height)
        {
            EndLevel();
            _state.NbLevel++;
            StartNewLevel();
        }
        else
        {
            ResolveCell();
        }
    }

    private void HandleMovementLeft()
    {
        if (_state.Hero.MoveConstraint == MovementConstraint.Left)
        {
            // check if the hero can move left
            _output.AddContextMessage(LanguageKey.CannotMoveLeft);
            _output.DisplayScreen();
        }
        else if (_state.Hero.X - 1 < 0)
        {
            // check array bounds
            _output.AddContextMessage(LanguageKey.CannotMoveThere);
            _output.DisplayScreen();
        }
        else
        {
            _state.Hero.MoveLeft();
            ResolveCell();
        }
    }

    private void HandleMovementRight()
    {
        if (_state.Hero.MoveConstraint == MovementConstraint.Right)
        {
            // check if the hero can move right
            _output.AddContextMessage(LanguageKey.CannotMoveRight);
            _output.DisplayScreen();
        }
        else if (_state.Hero.X + 1 >= _state.Dungeon.Width)
        {
            // check array bounds
            _output.AddContextMessage(LanguageKey.CannotMoveThere);
            _output.DisplayScreen();
        }
        else
        {
            _state.Hero.MoveRight();
            ResolveCell();
        }
    }

    private void ResolveCell()
    {
        Cell cell = _state.Dungeon.Grid[_state.Hero.Y, _state.Hero.X];

        switch (cell.Type)
        {
            case CellType.Monster:
                int monsterStrength = cell.Value;
                _state.Hero.DecreaseHealth(monsterStrength);
                ClearCell(cell, _state.Hero.Y, _state.Hero.X);
                _output.SetState(_state);
                _output.AddContextMessage(LanguageKey.MonsterEncounter, monsterStrength);
                _output.DisplayScreen();
                break;
            case CellType.Treasure:
                int treasureValue = cell.Value;
                _state.Hero.IncreaseScore(treasureValue);
                ClearCell(cell, _state.Hero.Y, _state.Hero.X);
                _output.SetState(_state);
                _output.AddContextMessage(LanguageKey.TreasureFound, treasureValue);
                _output.DisplayScreen();
                break;
            case CellType.Empty:
                _output.SetState(_state);
                _output.DisplayScreen(false);
                break;
        }
        
        if (_state.Hero.Y == _state.Dungeon.Height - 1)
        {
            _output.AddContextMessage(LanguageKey.LevelEnd);
            _output.DisplayScreen();
        }
    }
    
    private void GameOver()
    {
        _output.AddContextMessage(LanguageKey.GameOver);
        _output.DisplayScreen();
        Environment.Exit(0);
    }
    
    private void QuitGame()
    {
        _output.AddContextMessage(LanguageKey.ThanksForPlaying);
        _output.DisplayScreen();
        Environment.Exit(0);
    }

    private void ShowHint()
    {
        if (_state.Hero.NbHint > 0)
        {
            _state.Hero.DecreaseHint();
            var path = Algorithms.DP.PerfectSolution(_state);
            _output.AddStatusMessage(LanguageKey.PerfectPath, path);
            _output.AddContextMessage(LanguageKey.HintUsed);
        }
        else
        {
            _output.AddContextMessage(LanguageKey.NoHintAvailable);
        }

        _output.DisplayScreen();
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
        _output.AddContextMessage(LanguageKey.LevelCompleted);
        _output.AddContextMessage(LanguageKey.YourScore, _state.Hero.Score);

        if (_state.Hero.Score > _state.Dungeon.ScoreToBeat)
        {
            _state.Hero.AddHint();
            _output.AddContextMessage(LanguageKey.BeatScore);
        }
        else
        {
            _output.AddContextMessage(LanguageKey.DidNotBeatScore);
        }
        
        _output.DisplayScreen();
    }
}