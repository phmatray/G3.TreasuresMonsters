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
        _output.DisplayScreen();
        
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
        // check if the hero can move left
        if (_state.Hero.MoveConstraint == MovementConstraint.Left)
        {
            _output.AddStatusMessage(LanguageKey.CannotMoveLeft);
            return;
        }
        
        // check array bounds
        if (_state.Hero.X - 1 < 0)
        {
            _output.AddStatusMessage(LanguageKey.CannotMoveThere);
            return;
        }
        
        _state.Hero.MoveLeft();
        ResolveCell();
    }
    
    private void HandleMovementRight()
    {
        if (_state.Hero.MoveConstraint == MovementConstraint.Right)
        {
            // check if the hero can move right
            _output.AddStatusMessage(LanguageKey.CannotMoveRight);
        }
        else if (_state.Hero.X + 1 >= _state.Dungeon.Width)
        {
            // check array bounds
            _output.AddStatusMessage(LanguageKey.CannotMoveThere);
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
                break;
            case CellType.Treasure:
                int treasureValue = cell.Value;
                _state.Hero.IncreaseScore(treasureValue);
                ClearCell(cell, _state.Hero.Y, _state.Hero.X);
                _output.SetState(_state);
                _output.AddContextMessage(LanguageKey.TreasureFound, treasureValue);
                break;
            case CellType.Empty:
                _output.SetState(_state);
                _output.DisplayScreen();
                break;
        }
    }
    
    private void GameOver()
    {
        _output.AddContextMessage(LanguageKey.GameOver);
        Environment.Exit(0);
    }
    
    private void QuitGame()
    {
        _output.AddContextMessage(LanguageKey.ThanksForPlaying);
        Environment.Exit(0);
    }

    private void ShowHint()
    {
        if (_state.Hero.NbHint <= 0)
        {
            _output.AddStatusMessage(LanguageKey.NoHintAvailable);
        }
        else
        {
            _state.Hero.DecreaseHint();
            _output.AddStatusMessage(LanguageKey.CalculatingPerfectSolution);
            var path = Algorithms.DP.PerfectSolution(_state);
            _output.AddStatusMessage(LanguageKey.PerfectPath, path);
        }
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
        _output.AddStatusMessage(LanguageKey.LevelCompleted);
        _output.AddStatusMessage(LanguageKey.YourScore, _state.Hero.Score);

        if (_state.Hero.Score > _state.Dungeon.ScoreToBeat)
        {
            _state.Hero.AddHint();
            _output.AddStatusMessage(LanguageKey.BeatScore);
        }
        else
        {
            _output.AddStatusMessage(LanguageKey.DidNotBeatScore);
        }
    }
}