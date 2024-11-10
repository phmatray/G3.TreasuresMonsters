namespace G3.TreasuresMonsters.Features.Logic.Models;

public record HeroState(
    int X,
    int Y,
    int Health,
    int Score,
    MovementConstraint MoveConstraint);