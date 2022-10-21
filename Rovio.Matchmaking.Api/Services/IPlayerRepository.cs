using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Services;

public interface IPlayerRepository
{
    Task AddPlayer(PlayerModel player);
    Task<PlayerModel> GetNextPlayer();
}