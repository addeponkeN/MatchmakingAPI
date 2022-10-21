using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api;

public static class ModelExtensions
{
    public static MatchmakePlayer ToMatchmakePlayer(this PlayerModel model)
    {
        return new()
        {
            Id = model.Id,
            Rank = model.Rank,
            Region = model.Region
        };
    }
}