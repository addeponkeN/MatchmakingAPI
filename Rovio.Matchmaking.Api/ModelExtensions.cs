using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api;

public static class ModelExtensions
{
    public static Player ToMatchmakePlayer(this PlayerModel model)
    {
        return new()
        {
            Key = model.Key,
            Rank = model.Rank,
            Continent = model.Continent
        };
    }

    public static PlayerModel ToPlayerModel(this Player player)
    {
        return new()
        {
            Key = player.Key,
            Rank = player.Rank,
            Continent = player.Continent
        };
    }

    public static List<PlayerModel> ToPlayerModels(this IEnumerable<Player> list)
    {
        return list.Select(item => item.ToPlayerModel()).ToList();
    }

    public static SessionModel ToModel(this Session session)
    {
        return new SessionModel()
        {
            Key = session.Id,
            Players = session.Players.ToPlayerModels()
        };
    }

    public static IEnumerable<SessionModel> ToModels(this IEnumerable<Session> sessions)
    {
        return sessions.Select(session => session.ToModel());
    }
}