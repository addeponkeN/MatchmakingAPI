using System.Net.Http.Headers;
using System.Net.Http.Json;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Client;

public class MatchmakingClient
{
    public ValidatedServerModel GetServerInfo => _validatedServer;

    private HttpClient _client;
    private ValidatedServerModel _validatedServer;
    private string _matchmakingRoute;
    private string _gameServiceRoute;

    public MatchmakingClient(string address, int port)
    {
        _client = new()
        {
            BaseAddress = new Uri($"https://{address}:{port}"),
            Timeout = TimeSpan.FromSeconds(30),
        };

        _matchmakingRoute = "api/v1/Matchmaking/";
        _gameServiceRoute = "api/v1/GameServices/";

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<HttpResponseMessage> RegisterServer(ServerModel server)
    {
        var response = await _client.PostAsJsonAsync(
            $"{_gameServiceRoute}register/{server}", server);

        if(response.IsSuccessStatusCode)
        {
            _validatedServer = await response.Content.ReadFromJsonAsync<ValidatedServerModel>();
        }

        return response;
    }

    /// <summary>
    /// Add a player to the matchmaking queue
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayer(PlayerModel player)
    {
        return await _client.PostAsJsonAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/players/add/{player}", player);
    }

    /// <summary>
    /// Add a group of players to the matchmaking queue
    /// </summary>
    /// <param name="groupModel">Group to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayers(PlayerGroupModel groupModel)
    {
        return await _client.PostAsJsonAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/players/addrange/{groupModel}", groupModel);
    }

    /// <summary>
    /// Get all ready matches 
    /// </summary>
    /// <returns></returns>
    public async Task<ReadySessionsModel> GetReadySessions()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/sessions/{2}");

        ReadySessionsModel sessions = null;

        if(response.IsSuccessStatusCode)
        {
            sessions = await response.Content.ReadFromJsonAsync<ReadySessionsModel>();
        }

        return sessions;
    }
}