using System.Net.Http.Headers;
using System.Net.Http.Json;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Client;

public class MatchmakingClient
{
    private HttpClient _client;

    private string _baseRoute;

    public MatchmakingClient(string address, int port)
    {
        _client = new()
        {
            BaseAddress = new Uri($"https://{address}:{port}"),
            Timeout = TimeSpan.FromSeconds(5),
        };

        _baseRoute = "api/v1/Matchmaking/";

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Add a player to the matchmaking queue
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayer(int game, PlayerModel player)
    {
        return await _client.PostAsJsonAsync(
            $"{_baseRoute}{game}/players/add/{player}", player);
    }

    /// <summary>
    /// Add a group of players to the matchmaking queue
    /// </summary>
    /// <param name="groupModel">Group to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayers(PlayerGroupModel groupModel)
    {
        return await _client.PostAsJsonAsync(
            $"{_baseRoute}players/addrange/{groupModel}", groupModel);
    }

    /// <summary>
    /// Get all ready matches 
    /// </summary>
    /// <returns></returns>
    public async Task<ReadySessionsModel> GetReadySessions()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{_baseRoute}sessions/{2}");

        ReadySessionsModel sessions = null;

        Console.WriteLine($"GetStatus sessions: {response.StatusCode}");
        if(response.IsSuccessStatusCode)
        {
            sessions = await response.Content.ReadFromJsonAsync<ReadySessionsModel>();
        }

        return sessions;
    }
}