namespace G3.TreasuresMonsters.Features.Logic;

public static partial class Algorithms
{
    public class DPState(
        int heroX,
        int heroY,
        MovementConstraint moveConstraint,
        int heroHealth,
        int heroScore)
    {
        public int HeroX { get; } = heroX;
        public int HeroY { get; } = heroY;
        public MovementConstraint MoveConstraint { get; } = moveConstraint;
        public int HeroHealth { get; } = heroHealth;
        public int HeroScore { get; } = heroScore;

        // Override Equals and GetHashCode for correct dictionary behavior
        public override bool Equals(object? obj) =>
            obj is DPState other &&
            HeroX == other.HeroX &&
            HeroY == other.HeroY &&
            MoveConstraint == other.MoveConstraint &&
            HeroHealth == other.HeroHealth;

        public override int GetHashCode() => 
            HashCode.Combine(HeroX, HeroY, MoveConstraint, HeroHealth);

        public override string ToString() =>
            $"({HeroX}, {HeroY}, {MoveConstraint}, {HeroHealth}, {HeroScore})";
    }

    /* --- Dynamic Programming --- */
    public static class DP
    {
        // Signature de la méthode en Java :
        // String perfectSolution(State state)
        // https://algodaily.com/lessons/memoization-in-dynamic-programming/csharp
        public static string PerfectSolution(State initialState)
        {
            // Check if the hero is dead
            if (initialState.HeroHealth <= 0)
                return "The hero is already dead.";
            
            var dp = new Dictionary<DPState, (int totalScore, DPState? predecessor, string? move)>();
            var queue = new Queue<DPState>();

            // Initialize starting state
            var startState = new DPState(
                initialState.HeroX,
                initialState.HeroY,
                MovementConstraint.None,
                initialState.HeroHealth,
                0
            );

            dp[startState] = (initialState.HeroScore, null, null);
            queue.Enqueue(startState);

            int maxTotalScore = 0;
            DPState? bestEndState = null;

            while (queue.Count > 0)
            {
                var currentState = queue.Dequeue();

                // Check if the hero has exited the dungeon
                if (currentState.HeroY >= initialState.DungeonHeight)
                {
                    // Update the best end state if necessary
                    if (currentState.HeroScore + currentState.HeroHealth > maxTotalScore)
                    {
                        maxTotalScore = currentState.HeroScore + currentState.HeroHealth;
                        bestEndState = currentState;
                    }

                    continue;
                }

                // Generate possible moves
                foreach (var move in Constants.GetMoves()) // ["↓", "←", "→"]
                {
                    // Check movement constraints
                    if (move == Constants.MoveLeft && currentState.MoveConstraint == MovementConstraint.Left)
                        continue; // Cannot move left due to constraint
                    if (move == Constants.MoveRight && currentState.MoveConstraint == MovementConstraint.Right)
                        continue; // Cannot move right due to constraint

                    // Determine new position and movement constraint
                    int newX = currentState.HeroX;
                    int newY = currentState.HeroY;
                    MovementConstraint newMoveConstraint = currentState.MoveConstraint;

                    switch (move)
                    {
                        case Constants.MoveDown:
                            newY += 1;
                            newMoveConstraint = MovementConstraint.None;
                            break;
                        case Constants.MoveLeft:
                            newX -= 1;
                            newMoveConstraint = MovementConstraint.Right;
                            break;
                        case Constants.MoveRight:
                            newX += 1;
                            newMoveConstraint = MovementConstraint.Left;
                            break;
                    }

                    // Check if new position is within bounds
                    if (newX < 0 || newX >= initialState.DungeonWidth)
                        continue;

                    // Get new health and score
                    int newHealth = currentState.HeroHealth;
                    int newScore = currentState.HeroScore;

                    // Copy the monsters and treasures arrays
                    var newMonsters = initialState.CopyArray(initialState.Monsters);
                    var newTreasures = initialState.CopyArray(initialState.Treasures);

                    // Check for out-of-bounds (after moving down from the bottom row)
                    if (newY < initialState.DungeonHeight)
                    {
                        // Resolve cell
                        int cellMonster = newMonsters[newY][newX];
                        int cellTreasure = newTreasures[newY][newX];

                        if (cellMonster > 0)
                        {
                            newHealth -= cellMonster;
                            newMonsters[newY][newX] = 0;
                        }

                        if (cellTreasure > 0)
                        {
                            newScore += cellTreasure;
                            newTreasures[newY][newX] = 0;
                        }

                        if (newHealth <= 0)
                            continue; // Hero dies, skip this path
                    }

                    // Create new state
                    var newState = new DPState(newX, newY, newMoveConstraint, newHealth, newScore);

                    // Check if this state is better than any previously recorded state at this position with the same movement constraint
                    if (dp.TryGetValue(newState, out var existingState))
                    {
                        if (newScore <= existingState.totalScore)
                            continue; // Existing state is better or equal
                    }

                    // Update DP table
                    dp[newState] = (newScore, currentState, move);
                    queue.Enqueue(newState);
                }
            }

            // If no valid path found
            if (bestEndState == null)
                return "There is no valid path.";

            // Reconstruct the path from the best end state
            var pathMoves = new Stack<string>();
            var state = bestEndState;

            while (dp[state].predecessor != null)
            {
                var move = dp[state].move;
                pathMoves.Push(move);
                state = dp[state].predecessor;
            }

            return string.Join("", pathMoves);
        }
    }
}