using G3.TreasuresMonsters.Features.Logic.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Greedy Search --- */
    public static class GS
    {
        public const int DefaultDepthLimit = 5;
        
        // Signature of the method:
        // int GreedySolution(State state)
        public static int GreedySolution(State state)
        {
            var hero = new HeroState(state.HeroX, state.HeroY, state.HeroHealth, 0, MovementConstraint.None);
            int remainingDepth = DefaultDepthLimit; // Set the total depth limit

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

                var newHeroState = GetUpdatedState(state, hero, positionResult.X, positionResult.Y, positionResult.MoveConstraint);

                if (newHeroState.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, newHeroState, remainingDepth - 1);

                if (value > bestValue)
                {
                    bestValue = value;
                    bestHeroState = newHeroState;
                }
            }

            return bestHeroState;
        }

        // Function to evaluate the position with limited depth
        private static int EvaluatePosition(
            State state,
            HeroState hero,
            int depth)
        {
            if (depth == 0 || hero.Health <= 0 || hero.Y >= state.DungeonHeight)
            {
                return hero.Health <= 0 ? 0 : hero.Score;
            }

            int maxValue = hero.Score;

            var moves = Constants.GetMoves();
            for (var index = 0; index < moves.Length; index++)
            {
                var move = moves[index];
                if (!IsValidMove(hero.MoveConstraint, move))
                    continue;

                var positionResult = GetNewPositionAndConstraint(hero.X, hero.Y, hero.MoveConstraint, move);

                if (!IsValidPosition(state, positionResult))
                    continue;

                var newHeroState = GetUpdatedState(state, hero, positionResult.X, positionResult.Y,
                    positionResult.MoveConstraint);

                if (newHeroState.Health <= 0)
                    continue;

                int value = EvaluatePosition(state, newHeroState, depth - 1);

                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return maxValue;
        }
    }
}