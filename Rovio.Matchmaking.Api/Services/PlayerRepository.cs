using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api.Services;

public class PlayerRepository : IPlayerRepository
{
    public IEnumerable<PlayerModel> Players => _players;

    private Queue<Models.PlayerModel> _players;

    public PlayerRepository()
    {
        _players = new();
        AddPlayer(new() {Name = "olaf", Region = Regions.EU});
        AddPlayer(new() {Name = "john", Region = Regions.NA});
    }

    public Task AddPlayer(Models.PlayerModel player)
    {
        _players.Enqueue(player);
        return Task.CompletedTask;
    }

    public Task<PlayerModel> GetNextPlayer()
    {
        return Task.FromResult(_players.Dequeue());
    }
}