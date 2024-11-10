namespace G3.TreasuresMonsters.Features.Logic;

// Definition of records for results
public record GSPositionResult(
    int X,
    int Y,
    MovementConstraint MoveConstraint);

public record GSStateResult(
    int Health,
    int Score);

public static partial class Algorithms
{
    /* --- Greedy Search --- */
    public static class GS
    {
        // Signature of the method:
        // int GreedySolution(State state)
        public static int GreedySolution(State state)
        {
            int heroX = state.HeroX;
            int heroY = state.HeroY;
            int heroHealth = state.HeroHealth;
            int heroScore = 0; // The hero has not collected any treasures yet
            MovementConstraint moveConstraint = MovementConstraint.None;

            int remainingDepth = 5; // Set the total depth limit

            // While the hero hasn't reached the end and is alive, and depth limit not reached
            while (heroY < state.DungeonHeight && heroHealth > 0 && remainingDepth > 0)
            {
                int bestValue = int.MinValue;
                string? bestMove = null;
                int bestNewX = heroX;
                int bestNewY = heroY;
                int bestNewHealth = heroHealth;
                int bestNewScore = heroScore;
                MovementConstraint bestMoveConstraint = moveConstraint;

                // Evaluate each possible move (down, left, right)
                foreach (var move in Constants.GetMoves())
                {
                    if (!IsValidMove(moveConstraint, move))
                        continue;

                    var positionResult = GetNewPositionAndConstraint(heroX, heroY, moveConstraint, move);

                    if (positionResult.X < 0 || positionResult.X >= state.DungeonWidth || positionResult.Y >= state.DungeonHeight)
                        continue;

                    var stateResult = GetUpdatedState(state, positionResult.X, positionResult.Y, heroHealth, heroScore);

                    if (stateResult.Health <= 0)
                        continue;

                    // Evaluate the position by exploring paths within the remaining depth
                    int value = EvaluatePosition(state, positionResult.X, positionResult.Y, stateResult.Health, stateResult.Score, remainingDepth - 1, positionResult.MoveConstraint);

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                        bestNewX = positionResult.X;
                        bestNewY = positionResult.Y;
                        bestNewHealth = stateResult.Health;
                        bestNewScore = stateResult.Score;
                        bestMoveConstraint = positionResult.MoveConstraint;
                    }
                }

                if (bestMove == null)
                {
                    // No valid moves, the hero is blocked or dead
                    break;
                }

                // Update the hero's position and state
                heroX = bestNewX;
                heroY = bestNewY;
                heroHealth = bestNewHealth;
                heroScore = bestNewScore;
                moveConstraint = bestMoveConstraint;

                remainingDepth--; // Decrease the remaining depth as we have made a move

                // You can record the move if necessary
                // For example: path += bestMove;
            }

            // Return the total treasures collected
            return heroScore;
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
                if (health <= 0)
                    return int.MinValue; // The hero is dead
                else
                    return score; // Total treasures collected
            }

            int maxValue = int.MinValue;

            foreach (var move in Constants.GetMoves())
            {
                if (!IsValidMove(moveConstraint, move))
                    continue;

                var positionResult = GetNewPositionAndConstraint(x, y, moveConstraint, move);

                if (positionResult.X < 0 || positionResult.X >= state.DungeonWidth || positionResult.Y >= state.DungeonHeight)
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

            if (maxValue == int.MinValue)
            {
                // No valid moves
                if (health <= 0)
                    return int.MinValue;
                else
                    return score;
            }

            return maxValue;
        }

        // Checks if the move is valid based on movement constraints
        private static bool IsValidMove(MovementConstraint moveConstraint, string move)
        {
            return !(move == Constants.MoveLeft && moveConstraint == MovementConstraint.Left) &&
                   !(move == Constants.MoveRight && moveConstraint == MovementConstraint.Right);
        }

        // Calculates the new position and movement constraint
        private static GSPositionResult GetNewPositionAndConstraint(
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

            return new GSPositionResult(newX, newY, newMoveConstraint);
        }

        // Updates the hero's health and score based on the new position
        private static GSStateResult GetUpdatedState(
            State state,
            int x,
            int y,
            int health,
            int score)
        {
            if (y >= state.DungeonHeight)
            {
                return new GSStateResult(health, score);
            }

            int newHealth = health - Math.Max(0, state.Monsters[y][x]);
            int newScore = score + Math.Max(0, state.Treasures[y][x]);

            return new GSStateResult(newHealth, newScore);
        }
    }
}