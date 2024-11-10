using G3.TreasuresMonsters.Features.Logic.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    public static int Seed { get; private set; } = 0;

    public static Random Rng { get; private set; } = Seed == 0 ? new Random() : new Random(Seed);

    public static void SetSeed(int seed)
    {
        Seed = seed;
        Rng = new Random(Seed);
    }

    // Checks if the move is valid based on movement constraints
    public static bool IsValidMove(MovementConstraint moveConstraint, string move)
    {
        return !(move == Constants.MoveLeft && moveConstraint == MovementConstraint.Left) &&
               !(move == Constants.MoveRight && moveConstraint == MovementConstraint.Right);
    }

    // Checks if the position is valid within the dungeon
    public static bool IsValidPosition(State state, PositionResult positionResult)
    {
        return positionResult.X >= 0 && positionResult.X < state.DungeonWidth &&
               positionResult.Y < state.DungeonHeight;
    }

    // Calculates the new position and movement constraint
    public static PositionResult GetNewPositionAndConstraint(
        int x,
        int y,
        MovementConstraint moveConstraint,
        string move)
    {
        int newX = x;
        int newY = y;
        MovementConstraint newMoveConstraint = moveConstraint;

        switch (move)
        {
            case Constants.MoveDown:
                newY++;
                newMoveConstraint = MovementConstraint.None;
                break;
            case Constants.MoveLeft:
                newX--;
                newMoveConstraint = MovementConstraint.Right;
                break;
            case Constants.MoveRight:
                newX++;
                newMoveConstraint = MovementConstraint.Left;
                break;
        }

        return new PositionResult(newX, newY, newMoveConstraint);
    }

    // Updates the hero's health and score based on the new position
    public static HeroStateUpdateResult GetUpdatedState(
        State state,
        int x,
        int y,
        int health,
        int score)
    {
        if (y >= state.DungeonHeight)
        {
            return new HeroStateUpdateResult(health, score);
        }

        int newHealth = health - Math.Max(0, state.Monsters[y][x]);
        int newScore = score + Math.Max(0, state.Treasures[y][x]);

        return new HeroStateUpdateResult(newHealth, newScore);
    }

    // Overload for DP algorithm, using current HeroState
    public static HeroStateUpdateResult GetUpdatedState(
        State state,
        HeroState currentState,
        int newX,
        int newY)
    {
        if (newY >= state.DungeonHeight)
        {
            return new HeroStateUpdateResult(currentState.Health, currentState.Score);
        }

        int newHealth = currentState.Health - Math.Max(0, state.Monsters[newY][newX]);
        int newScore = currentState.Score + Math.Max(0, state.Treasures[newY][newX]);

        return new HeroStateUpdateResult(newHealth, newScore);
    }
}