namespace Rovio.Matchmaking.Models;

public record ReadyMatchesModel
{
    public IEnumerable<MatchModel> Matches { get; init; }
}