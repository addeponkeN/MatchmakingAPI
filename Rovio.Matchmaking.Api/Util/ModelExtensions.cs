namespace Rovio.Matchmaking.Api.Util;

public static class ModelExtensions
{
    public static MatchmakingPlayer ToMatchmakePlayer(this Models.Player model, MatchmakingManager mm)
    {
        var player = mm.CreatePlayer();
        player.Set(model.UniqueId, model.Continent, model.Rank);
        return player;
    }

    public static Models.Player ToPlayerModel(this MatchmakingPlayer player)
    {
        //  todo - pooling
        return new()
        {
            UniqueId = player.UniqueId,
            Rank = player.Rank,
            Continent = player.Continent
        };
    }

    public static List<Models.Player> ToPlayerModels(this IEnumerable<MatchmakingPlayer> list)
    {
        var retList = new List<Models.Player>();
        foreach(var p in list)
            retList.Add(p.ToPlayerModel());
        return retList;
    }

    public static Models.Session ToModel(this MatchmakingSession matchmakingSession)
    {
        return new Models.Session()
        {
            Key = matchmakingSession.Id,
            Players = matchmakingSession.Players.ToPlayerModels()
        };
    }

    public static IEnumerable<Models.Session> ToModels(this IEnumerable<MatchmakingSession> sessions)
    {
        return sessions.Select(session => session.ToModel());
    }
}