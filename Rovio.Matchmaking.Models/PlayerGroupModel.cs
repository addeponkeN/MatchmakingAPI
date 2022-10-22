namespace Rovio.Matchmaking.Models;

/// <summary>
/// A collection of player models
/// </summary>
public record PlayerGroupModel
{
    public IEnumerable<PlayerModel> Members { get; init; }
}