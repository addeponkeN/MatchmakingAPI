using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api;

public static class ModelExtensions
{
    public static Player ToMatchmakePlayer(this Models.Player model)
    {
        return new()
        {
            Key = model.Key,
            Rank = model.Rank,
            Continent = model.Continent
        };
    }

    public static Models.Player ToPlayerModel(this Player player)
    {
        return new()
        {
            Key = player.Key,
            Rank = player.Rank,
            Continent = player.Continent
        };
    }

    public static List<Models.Player> ToPlayerModels(this IEnumerable<Player> list)
    {
        return list.Select(item => item.ToPlayerModel()).ToList();
    }

    public static Models.Session ToModel(this Session session)
    {
        return new Models.Session()
        {
            Key = session.Id,
            Players = session.Players.ToPlayerModels()
        };
    }

    public static IEnumerable<Models.Session> ToModels(this IEnumerable<Session> sessions)
    {
        return sessions.Select(session => session.ToModel());
    }
}