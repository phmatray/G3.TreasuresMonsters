using G3.TreasuresMonsters.Features.Logic.Models;

namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature of the method with Java :
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
            var moves = Constants.GetMoves();
            for (var index = 0; index < moves.Length; index++)
            {
                var move = moves[index];
                if (!IsValidMove(currentState.MoveConstraint, move))
                    continue;

                var positionResult =
                    GetNewPositionAndConstraint(currentState.X, currentState.Y, currentState.MoveConstraint, move);

                if (positionResult.X < 0 || positionResult.X >= initialState.DungeonWidth)
                    continue;

                var newHeroState = GetUpdatedState(initialState, currentState, positionResult.X, positionResult.Y,
                    positionResult.MoveConstraint);

                if (newHeroState.Health <= 0)
                    continue;

                bool isBetterScore =
                    !dp.TryGetValue(newHeroState, out var existingState) ||
                    newHeroState.Score > existingState.TotalScore;

                if (isBetterScore)
                {
                    dp[newHeroState] = new DynamicProgramingRecord(newHeroState.Score, currentState, move);
                    queue.Enqueue(newHeroState);
                }
            }
        }

        private static string ReconstructPath(Dictionary<HeroState, DynamicProgramingRecord> dp, HeroState bestEndState)
        {
            return dp[bestEndState].Predecessor != null
                ? ReconstructPath(dp, dp[bestEndState].Predecessor) + dp[bestEndState].Move
                : string.Empty;
        }
    }
}