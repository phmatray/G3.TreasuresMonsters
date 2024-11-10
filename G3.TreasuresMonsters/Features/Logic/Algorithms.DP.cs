using G3.TreasuresMonsters.Features.Logic.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature de la méthode en Java :
        // String perfectSolution(State state)
        // https://algodaily.com/lessons/memoization-in-dynamic-programming/csharp
        public static string PerfectSolution(State initialState)
        {
            if (initialState.HeroHealth <= 0)
                return "<DEAD>";

            var dp = new Dictionary<HeroState, DynamicProgramingRecord>();
            var queue = new Queue<HeroState>();

            InitializeStartState(initialState, dp, queue);

            var result = ProcessQueue(initialState, dp, queue);

            return result == null
                ? "<INVALID>"
                : ReconstructPath(dp, result);
        }

        private static void InitializeStartState(
            State initialState,
            Dictionary<HeroState, DynamicProgramingRecord> dp,
            Queue<HeroState> queue)
        {
            var startState = new HeroState(
                initialState.HeroX,
                initialState.HeroY,
                initialState.HeroHealth,
                0, MovementConstraint.None);

            dp[startState] = new DynamicProgramingRecord(initialState.HeroScore, null, null);
            queue.Enqueue(startState);
        }

        private static HeroState? ProcessQueue(
            State initialState,
            Dictionary<HeroState, DynamicProgramingRecord> dp,
            Queue<HeroState> queue)
        {
            int highestScoreAchieved = 0;
            HeroState? bestEndState = null;

            while (queue.Count > 0)
            {
                var currentState = queue.Dequeue();

                if (currentState.Y >= initialState.DungeonHeight)
                {
                    int currentTotalScore = currentState.Score + currentState.Health;
                    if (currentTotalScore > highestScoreAchieved)
                    {
                        highestScoreAchieved = currentTotalScore;
                        bestEndState = currentState;
                    }
                    continue;
                }

                GenerateAndProcessMoves(initialState, currentState, dp, queue);
            }

            return bestEndState;
        }

        private static void GenerateAndProcessMoves(
            State initialState,
            HeroState currentState,
            Dictionary<HeroState, DynamicProgramingRecord> dp,
            Queue<HeroState> queue)
        {
            foreach (var move in Constants.GetMoves())
            {
                if (!IsValidMove(currentState.MoveConstraint, move))
                    continue;

                var (newX, newY, newMoveConstraint) = GetNewPositionAndConstraint(currentState, move);

                if (newX < 0 || newX >= initialState.DungeonWidth)
                    continue;

                var (newHealth, newScore) = GetUpdatedState(initialState, currentState, newX, newY);

                if (newHealth <= 0)
                    continue;

                var newState = new HeroState(newX, newY, newHealth, newScore, newMoveConstraint);

                bool isBetterScore =
                    !dp.TryGetValue(newState, out var existingState) ||
                    newScore > existingState.TotalScore;
                
                if (isBetterScore)
                {
                    dp[newState] = new DynamicProgramingRecord(newScore, currentState, move);
                    queue.Enqueue(newState);
                }
            }
        }

        private static bool IsValidMove(MovementConstraint moveConstraint, string move)
        {
            return !(move == Constants.MoveLeft && moveConstraint == MovementConstraint.Left) &&
                   !(move == Constants.MoveRight && moveConstraint == MovementConstraint.Right);
        }

        private static PositionResult GetNewPositionAndConstraint(HeroState currentState, string move)
        {
            var (newX, newY) = (HeroX: currentState.X, HeroY: currentState.Y);
            var newMoveConstraint = currentState.MoveConstraint;

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

        private static HeroStateUpdateResult GetUpdatedState(State initialState, HeroState currentState, int newX, int newY)
        {
            if (newY >= initialState.DungeonHeight)
            {
                return new HeroStateUpdateResult(currentState.Health, currentState.Score);
            }

            int newHealth = currentState.Health - Math.Max(0, initialState.Monsters[newY][newX]);
            int newScore = currentState.Score + Math.Max(0, initialState.Treasures[newY][newX]);

            return new HeroStateUpdateResult(newHealth, newScore);
        }

        private static string ReconstructPath(Dictionary<HeroState, DynamicProgramingRecord> dp, HeroState bestEndState)
        {
            return dp[bestEndState].Predecessor != null
                ? ReconstructPath(dp, dp[bestEndState].Predecessor) + dp[bestEndState].Move
                : string.Empty;
        }
    }
}