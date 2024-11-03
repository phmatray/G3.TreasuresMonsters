// ReSharper disable InconsistentNaming

namespace G3.TreasuresMonsters.Features.Engine;

public class GameEngine(
    IGameInput _input,
    IGameOutput _output)
{
    private State _state = null!;
    
    public void StartNewGame()
    {
        InitializeState();
        StartNewLevel();
    }

    private void InitializeState()
    {
        _state = new State(
            [0, 0],
            Constants.MaxHealth,
            0,
            [],
            [],
            1,
            0
        );
    }

    private void StartNewLevel()
    {
        _state.InitializeDungeon();
        
        _output.SetState(_state);
        _output.DisplayScreen(false);
        
        PlayLevel();
    }

    private void PlayLevel()
    {
        while (true)
        {
            if (_state.HeroIsDead)
            {
                HandleGameOver();
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
            case Constants.MoveUpKey:
                HandleMoveUp();
                break;
            case Constants.MoveDownKey:
                HandleMoveDown();
                break;
            case Constants.MoveLeftKey:
                HandleMoveLeft();
                break;
            case Constants.MoveRightKey:
                HandleMoveRight();
                break;
            case Constants.ShowHintKey:
                HandleShowHint();
                break;
            case Constants.QuitGameKey:
                HandleQuitGame();
                break;
            default:
                _output.AddStatusMessage(LanguageKey.InvalidInput);
                break;
        }
    }

    private void HandleMoveUp()
    {
        _output.AddContextMessage(LanguageKey.CannotMoveUp);
        _output.DisplayScreen();
    }
    
    private void HandleMoveDown()
    {
        var cellResolution = _state.MoveDown();
        ProcessCellResolution(cellResolution);
    }

    private void HandleMoveLeft()
    {
        if (_state.CanMoveLeft())
        {
            var cellResolution = _state.MoveLeft();
            ProcessCellResolution(cellResolution);
        }
        else
        {
            var cannotMoveKey = _state.HeroMoveConstraint == MovementConstraint.Left
                ? LanguageKey.CannotMoveLeft
                : LanguageKey.CannotMoveThere;
            
            _output.AddContextMessage(cannotMoveKey);
            _output.DisplayScreen();
        }
    }

    private void HandleMoveRight()
    {
        if (_state.CanMoveRight())
        {
            var cellResolution = _state.MoveRight();
            ProcessCellResolution(cellResolution);
        }
        else
        {
            var cannotMoveKey = _state.HeroMoveConstraint == MovementConstraint.Right
                ? LanguageKey.CannotMoveRight
                : LanguageKey.CannotMoveThere;
            
            _output.AddContextMessage(cannotMoveKey);
            _output.DisplayScreen();
        }
    }

    private void ProcessCellResolution(CellResolution cellResolution)
    {
        _output.SetState(_state);

        switch (cellResolution.Type)
        {
            case CellResolutionType.Empty:
                _output.DisplayScreen(false);
                break;
            case CellResolutionType.Monster:
                _output.AddContextMessage(LanguageKey.MonsterEncounter, cellResolution.Value);
                _output.DisplayScreen();
                break;
            case CellResolutionType.Treasure:
                _output.AddContextMessage(LanguageKey.TreasureFound, cellResolution.Value);
                _output.DisplayScreen();
                break;
            case CellResolutionType.EndOfLevel:
                EndLevel();
                StartNewLevel();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (_state.HeroY == _state.DungeonHeight - 1)
        {
            _output.AddContextMessage(LanguageKey.LevelEnd);
            _output.DisplayScreen();
        }
    }

    private void HandleShowHint()
    {
        if (_state.NbHint > 0)
        {
            _state.DecreaseHint();
            
            var path = Algorithms.DP.PerfectSolution(_state);
            
            switch (path)
            {
                // path <DEAD> means the hero is dead
                case "<DEAD>":
                    _output.AddStatusMessage(LanguageKey.HeroIsDead);
                    break;
                // path <INVALID> means the hero cannot reach the end of the dungeon
                case "<INVALID>":
                    _output.AddStatusMessage(LanguageKey.NoValidPath);
                    break;
                // path is a valid path. Simplify it and display it
                default:
                {
                    var simplifiedPath = new HeroPath(path).NormalizedPath;
                    _output.AddStatusMessage(LanguageKey.PerfectPath, simplifiedPath);
                    break;
                }
            }

            _output.AddContextMessage(LanguageKey.HintUsed);
        }
        else
        {
            _output.AddContextMessage(LanguageKey.NoHintAvailable);
        }

        _output.DisplayScreen();
    } 
    
    private static string SimplifyPath(string path)
    {
        // A path like
        // "↓←↓↓←↓←↓↓↓→→↓→→→↓→↓←↓";
        // should be simplified to
        // "↓ ← 2↓ ← ↓ ← 3↓ 2→ ↓ 3→ ↓ → ↓ ← ↓";
        
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        StringBuilder simplifiedPath = new StringBuilder();
        int count = 1;

        for (int i = 1; i < path.Length; i++)
        {
            if (path[i] == path[i - 1])
            {
                count++;
            }
            else
            {
                simplifiedPath.Append(path[i - 1]);
                if (count > 1)
                {
                    simplifiedPath.Append(count);
                }

                simplifiedPath.Append(' ');
                count = 1;
            }
        }

        // Append the last character and its count
        simplifiedPath.Append(path[^1]);
        if (count > 1)
        {
            simplifiedPath.Append(count);
        }

        return simplifiedPath.ToString().Trim();
    }

    private void HandleQuitGame()
    {
        _output.AddContextMessage(LanguageKey.ThanksForPlaying);
        _output.DisplayScreen();
        Environment.Exit(0);
    }
    
    private void HandleGameOver()
    {
        _output.AddContextMessage(LanguageKey.GameOver);
        _output.DisplayScreen();
        Environment.Exit(0);
    }
    
    private void EndLevel()
    {
        _output.AddContextMessage(LanguageKey.LevelCompleted);
        _output.AddContextMessage(LanguageKey.YourScore, _state.HeroScore);

        if (_state.HeroScore > _state.DungeonScoreToBeat)
        {
            _state.AddHint();
            _output.AddContextMessage(LanguageKey.BeatScore);
        }
        else
        {
            _output.AddContextMessage(LanguageKey.DidNotBeatScore);
        }
        
        _output.DisplayScreen();
    }
}