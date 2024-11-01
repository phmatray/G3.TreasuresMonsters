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
            1
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
        _state.MoveHeroDown();
            
        if (_state.HeroY >= Constants.DungeonHeight)
        {
            EndLevel();
            _state.IncreaseCurrentLevel();
            StartNewLevel();
        }
        else
        {
            ResolveCell();
        }
    }

    private void HandleMovementLeft()
    {
        if (_state.HeroMoveConstraint == MovementConstraint.Left)
        {
            // check if the hero can move left
            _output.AddContextMessage(LanguageKey.CannotMoveLeft);
            _output.DisplayScreen();
        }
        else if (_state.HeroX - 1 < 0)
        {
            // check array bounds
            _output.AddContextMessage(LanguageKey.CannotMoveThere);
            _output.DisplayScreen();
        }
        else
        {
            _state.MoveHeroLeft();
            ResolveCell();
        }
    }

    private void HandleMovementRight()
    {
        if (_state.HeroMoveConstraint == MovementConstraint.Right)
        {
            // check if the hero can move right
            _output.AddContextMessage(LanguageKey.CannotMoveRight);
            _output.DisplayScreen();
        }
        else if (_state.HeroX + 1 >= Constants.DungeonWidth)
        {
            // check array bounds
            _output.AddContextMessage(LanguageKey.CannotMoveThere);
            _output.DisplayScreen();
        }
        else
        {
            _state.MoveHeroRight();
            ResolveCell();
        }
    }

    private void ResolveCell()
    {
        if (_state.Monsters[_state.HeroY][_state.HeroX] > 0)
        {
            int monsterStrength = _state.Monsters[_state.HeroY][_state.HeroX];
            _state.DecreaseHeroHealth(monsterStrength);
            _state.Monsters[_state.HeroY][_state.HeroX] = 0;
            _output.SetState(_state);
            _output.AddContextMessage(LanguageKey.MonsterEncounter, monsterStrength);
            _output.DisplayScreen();
        }
        else if (_state.Treasures[_state.HeroY][_state.HeroX] > 0)
        {
            int treasureValue = _state.Treasures[_state.HeroY][_state.HeroX];
            _state.IncreaseHeroScore(treasureValue);
            _state.Treasures[_state.HeroY][_state.HeroX] = 0;
            _output.SetState(_state);
            _output.AddContextMessage(LanguageKey.TreasureFound, treasureValue);
            _output.DisplayScreen();
        }
        else
        {
            _output.SetState(_state);
            _output.DisplayScreen(false);
        }
        
        if (_state.HeroY == Constants.DungeonHeight - 1)
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
        if (_state.NbHint > 0)
        {
            _state.DecreaseHint();
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