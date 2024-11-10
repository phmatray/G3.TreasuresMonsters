using G3.TreasuresMonsters.Features.Logic.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Greedy Search --- */
    public static class GS
    {
        // Signature of the method:
        // int GreedySolution(State state)
        public static int GreedySolution(State state)
        {
            var hero = new HeroState(state.HeroX, state.HeroY, state.HeroHealth, 0, MovementConstraint.None);
            int remainingDepth = 5; // Set the total depth limit

            while (hero.Y < state.DungeonHeight && hero.Health > 0 && remainingDepth > 0)
            {
                var bestMove = FindBestMove(state, hero, remainingDepth);

                if (bestMove == null)
                {
                    // No valid moves, the hero is blocked or dead
                    break;
                }

                hero = bestMove;
                remainingDepth--; // Decrease the remaining depth as we have made a move
            }

            return hero.Score;
        }

        // Finds the best move for the hero given the current state and depth
        private static HeroState? FindBestMove(State state, HeroState hero, int remainingDepth)
        {
            int bestValue = 0;
            HeroState? bestHeroState = null;

            foreach (var move in Constants.GetMoves())
            {
                if (!IsValidMove(hero.MoveConstraint, move))
                    continue;

                var positionResult = GetNewPositionAndConstraint(hero.X, hero.Y, hero.MoveConstraint, move);

                if (!IsValidPosition(state, positionResult))
                    continue;

                var stateResult = GetUpdatedState(state, positionResult.X, positionResult.Y, hero.Health, hero.Score);

                if (stateResult.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, positionResult.X, positionResult.Y, stateResult.Health, stateResult.Score, remainingDepth - 1, positionResult.MoveConstraint);

                if (value > bestValue)
                {
                    bestValue = value;
                    bestHeroState = new HeroState(positionResult.X, positionResult.Y, stateResult.Health, stateResult.Score, positionResult.MoveConstraint);
                }
            }

            return bestHeroState;
        }

        // Function to evaluate the position with limited depth
        private static int EvaluatePosition(
            State state,
            int x,
            int y,
            int health,
            int score,
            int depth,
            MovementConstraint moveConstraint)
        {
            
            if (depth == 0 || health <= 0 || y >= state.DungeonHeight)
            {
                return health <= 0 ? 0 : score;
            }

            int maxValue = 0;

            foreach (var move in Constants.GetMoves())
            {
                if (!IsValidMove(moveConstraint, move))
                    continue;

                var positionResult = GetNewPositionAndConstraint(x, y, moveConstraint, move);

                if (!IsValidPosition(state, positionResult))
                    continue;

                var stateResult = GetUpdatedState(state, positionResult.X, positionResult.Y, health, score);

                if (stateResult.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, positionResult.X, positionResult.Y, stateResult.Health, stateResult.Score, depth - 1, positionResult.MoveConstraint);

                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return maxValue == 0 ? score : maxValue;
        }

        // Checks if the move is valid based on movement constraints
        private static bool IsValidMove(MovementConstraint moveConstraint, string move)
        {
            return !(move == Constants.MoveLeft && moveConstraint == MovementConstraint.Left) &&
                   !(move == Constants.MoveRight && moveConstraint == MovementConstraint.Right);
        }

        // Checks if the position is valid within the dungeon
        private static bool IsValidPosition(State state, PositionResult positionResult)
        {
            return positionResult.X >= 0 && positionResult.X < state.DungeonWidth && positionResult.Y < state.DungeonHeight;
        }
        
        // Calculates the new position and movement constraint
        private static PositionResult GetNewPositionAndConstraint(
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
        private static HeroStateUpdateResult GetUpdatedState(
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
    }
}