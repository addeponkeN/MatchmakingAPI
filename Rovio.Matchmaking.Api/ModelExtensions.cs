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

    public static MatchModel ToModel(this Session session)
    {
        return new MatchModel()
        {
            Key = session.MatchId,
            Players = session.Players.ToPlayerModels()
        };
    }

    public static IEnumerable<MatchModel> ToModels(this IEnumerable<Session> matches)
    {
        return matches.Select(match => match.ToModel());
    }


    // public static MatchDto ToDto(this Match match)
    // {
    // return new MatchDto()
    // {
    // Id = match.MatchId,
    // Players = match.Players.ToPlayerModels()
    // };
    // }

    // public static IEnumerable<MatchDto> ToDto(this IEnumerable<Match> matches)
    // {
    // return matches.Select(match => match.ToDto());
    // }
}