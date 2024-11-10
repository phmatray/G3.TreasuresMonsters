namespace G3.TreasuresMonsters.Features.Logic.Models;

public record DynamicProgramingRecord(
    int TotalScore,
    HeroState? Predecessor,
    string? Move);