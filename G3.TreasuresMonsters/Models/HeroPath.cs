namespace G3.TreasuresMonsters.Models;

public record HeroPath
{
    public HeroPath(string path)
    {
        Path = path;
        NormalizedPath = SimplifyPath(path);
    }

    public string Path { get; init; }
    
    public string NormalizedPath { get; }

    private static string SimplifyPath(string path)
    {
        // A path like
        // "↓←↓↓←↓←↓↓↓→→↓→→→↓→↓←↓";
        // should be simplified to
        // "↓ ← 2↓ ← ↓ ← 3↓ 2→ ↓ 3→ ↓ → ↓ ← ↓";
        
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        StringBuilder simplifiedPath = new StringBuilder();
        int count = 1;

        for (int i = 1; i < path.Length; i++)
        {
            if (path[i] == path[i - 1])
            {
                count++;
            }
            else
            {
                simplifiedPath.Append(path[i - 1]);
                if (count > 1)
                {
                    simplifiedPath.Append(count);
                }

                simplifiedPath.Append(' ');
                count = 1;
            }
        }

        // Append the last character and its count
        simplifiedPath.Append(path[^1]);
        if (count > 1)
        {
            simplifiedPath.Append(count);
        }

        return simplifiedPath.ToString().Trim();
    }
}