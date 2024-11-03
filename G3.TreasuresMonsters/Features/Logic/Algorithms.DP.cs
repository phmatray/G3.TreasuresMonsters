namespace G3.TreasuresMonsters.Features.Logic;

public record DPState(
    int HeroX,
    int HeroY,
    MovementConstraint MoveConstraint,
    int HeroHealth,
    int HeroScore);

public record DPRecord(
    int TotalScore,
    DPState? Predecessor,
    string? Move);

public record DPNewPositionResult(
    int NewX,
    int NewY,
    MovementConstraint NewMoveConstraint);

public record DPUpdatedStateResult(
    int NewHealth,
    int NewScore);

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

            var dp = new Dictionary<DPState, DPRecord>();
            var queue = new Queue<DPState>();

            InitializeStartState(initialState, dp, queue);

            var result = ProcessQueue(initialState, dp, queue);

            return result == null
                ? "<INVALID>"
                : ReconstructPath(dp, result);
        }

        private static void InitializeStartState(
            State initialState,
            Dictionary<DPState, DPRecord> dp,
            Queue<DPState> queue)
        {
            var startState = new DPState(
                initialState.HeroX,
                initialState.HeroY,
                MovementConstraint.None,
                initialState.HeroHealth,
                0
            );

            dp[startState] = new DPRecord(initialState.HeroScore, null, null);
            queue.Enqueue(startState);
        }

        private static DPState? ProcessQueue(
            State initialState,
            Dictionary<DPState, DPRecord> dp,
            Queue<DPState> queue)
        {
            int highestScoreAchieved = 0;
            DPState? bestEndState = null;

            while (queue.Count > 0)
            {
                var currentState = queue.Dequeue();

                if (currentState.HeroY >= initialState.DungeonHeight)
                {
                    int currentTotalScore = currentState.HeroScore + currentState.HeroHealth;
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
            DPState currentState,
            Dictionary<DPState, DPRecord> dp,
            Queue<DPState> queue)
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

                var newState = new DPState(newX, newY, newMoveConstraint, newHealth, newScore);

                bool isBetterScore =
                    !dp.TryGetValue(newState, out var existingState) ||
                    newScore > existingState.TotalScore;
                
                if (isBetterScore)
                {
                    dp[newState] = new DPRecord(newScore, currentState, move);
                    queue.Enqueue(newState);
                }
            }
        }

        private static bool IsValidMove(MovementConstraint moveConstraint, string move)
        {
            return !(move == Constants.MoveLeft && moveConstraint == MovementConstraint.Left) &&
                   !(move == Constants.MoveRight && moveConstraint == MovementConstraint.Right);
        }

        private static DPNewPositionResult GetNewPositionAndConstraint(DPState currentState, string move)
        {
            var (newX, newY) = (currentState.HeroX, currentState.HeroY);
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

            return new DPNewPositionResult(newX, newY, newMoveConstraint);
        }

        private static DPUpdatedStateResult GetUpdatedState(State initialState, DPState currentState, int newX, int newY)
        {
            if (newY >= initialState.DungeonHeight)
            {
                return new DPUpdatedStateResult(currentState.HeroHealth, currentState.HeroScore);
            }

            int newHealth = currentState.HeroHealth - Math.Max(0, initialState.Monsters[newY][newX]);
            int newScore = currentState.HeroScore + Math.Max(0, initialState.Treasures[newY][newX]);

            return new DPUpdatedStateResult(newHealth, newScore);
        }

        private static string ReconstructPath(Dictionary<DPState, DPRecord> dp, DPState bestEndState)
        {
            return dp[bestEndState].Predecessor != null
                ? ReconstructPath(dp, dp[bestEndState].Predecessor) + dp[bestEndState].Move
                : string.Empty;
        }
    }
}