using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

public record MatchModel
{
    public UniqueId UniqueId { get; init; }
    public List<PlayerModel> Players { get; init; }
}
