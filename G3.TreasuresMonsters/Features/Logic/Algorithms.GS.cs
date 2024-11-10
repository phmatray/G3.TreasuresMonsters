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

                var positionResult =
                    GetNewPositionAndConstraint(hero.X, hero.Y, hero.MoveConstraint, move);

                if (!IsValidPosition(state, positionResult))
                    continue;

                var stateResult = GetUpdatedState(state, positionResult.X, positionResult.Y,
                    hero.Health, hero.Score);

                if (stateResult.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, positionResult.X, positionResult.Y, stateResult.Health,
                    stateResult.Score, remainingDepth - 1, positionResult.MoveConstraint);

                if (value > bestValue)
                {
                    bestValue = value;
                    bestHeroState = new HeroState(positionResult.X, positionResult.Y, stateResult.Health,
                        stateResult.Score, positionResult.MoveConstraint);
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

            int maxValue = score; // Start with current score

            foreach (var move in Constants.GetMoves())
            {
                if (!IsValidMove(moveConstraint, move))
                    continue;

                var positionResult = GetNewPositionAndConstraint(x, y, moveConstraint, move);

                if (!IsValidPosition(state, positionResult))
                    continue;

                var stateResult =
                    GetUpdatedState(state, positionResult.X, positionResult.Y, health, score);

                if (stateResult.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, positionResult.X, positionResult.Y, stateResult.Health,
                    stateResult.Score, depth - 1, positionResult.MoveConstraint);

                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return maxValue;
        }
    }
}