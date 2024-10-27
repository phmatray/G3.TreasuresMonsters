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
        _state.Dungeon.ScoreToBeat = CalculateScoreToBeat();
        
        _output.ClearScreen();
        _output.DisplayBlankLine();
        _output.DisplayMessage(LanguageKey.Level, _state.NbLevel);
        _output.DisplayMessage(LanguageKey.ScoreToBeat, _state.Dungeon.ScoreToBeat);
        
        PlayLevel();
    }

    private int CalculateScoreToBeat()
    {
        return Algorithms.GS.GreedySolution(_state);
    }

    private void PlayLevel()
    {
        while (true)
        {
            _output.DisplayDungeon(_state.Dungeon, _state.Hero, _state.NbLevel, _state.Dungeon.ScoreToBeat);
            
            if (_state.Hero.IsDead)
            {
                GameOver();
            }

            _output.DisplayMessage(LanguageKey.MovePrompt);
            var inputKey = _input.GetInput();
            _output.DisplayBlankLine();

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
                _output.DisplayMessage(LanguageKey.InvalidInput);
                break;
        }
    }

    private void HandleMovementUp()
    {
        _output.DisplayMessage(LanguageKey.CannotMoveUp);
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
            _output.DisplayMessage(LanguageKey.CannotMoveLeft);
            return;
        }
        
        // check array bounds
        if (_state.Hero.X - 1 < 0)
        {
            _output.DisplayMessage(LanguageKey.CannotMoveThere);
            return;
        }
        
        _state.Hero.MoveLeft();
        ResolveCell();
    }
    
    private void HandleMovementRight()
    {
        // check if the hero can move right
        if (_state.Hero.MoveConstraint == MovementConstraint.Right)
        {
            _output.DisplayMessage(LanguageKey.CannotMoveRight);
            return;
        }
        
        // check array bounds
        if (_state.Hero.X + 1 >= _state.Dungeon.Width)
        {
            _output.DisplayMessage(LanguageKey.CannotMoveThere);
            return;
        }
        
        _state.Hero.MoveRight();
        ResolveCell();
    }

    private void ResolveCell()
    {
        Cell cell = _state.Dungeon.Grid[_state.Hero.Y, _state.Hero.X];

        switch (cell.Type)
        {
            case CellType.Monster:
                _state.Hero.DecreaseHealth(cell.Value);
                _output.DisplayMessage(LanguageKey.MonsterEncounter, cell.Value);
                ClearCell(cell, _state.Hero.Y, _state.Hero.X);
                break;
            case CellType.Treasure:
                _state.Hero.IncreaseScore(cell.Value);
                _output.DisplayMessage(LanguageKey.TreasureFound, cell.Value);
                ClearCell(cell, _state.Hero.Y, _state.Hero.X);
                break;
        }
    }
    
    private void GameOver()
    {
        _output.DisplayMessage(LanguageKey.GameOver);
        Environment.Exit(0);
    }
    
    private void QuitGame()
    {
        _output.DisplayMessage(LanguageKey.ThanksForPlaying);
        Environment.Exit(0);
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