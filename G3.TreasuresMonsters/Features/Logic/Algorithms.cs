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
}