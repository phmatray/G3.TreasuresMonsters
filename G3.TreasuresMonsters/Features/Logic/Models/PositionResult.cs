namespace G3.TreasuresMonsters.Features.Logic.Models;

public record PositionResult(
    int X,
    int Y,
    MovementConstraint MoveConstraint);