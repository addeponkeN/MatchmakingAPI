using System.Net.Http.Headers;
using System.Net.Http.Json;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Client;

public class MatchmakingClient
{
    private HttpClient _client;

    public MatchmakingClient(string address, int port)
    {
        _client = new()
        {
            BaseAddress = new Uri($"https://{address}:{port}"),
            Timeout = TimeSpan.FromSeconds(5),
        };

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Add a player to the matchmaking queue
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayer(PlayerModel player)
    {
        return await _client.PostAsJsonAsync(
            $"api/Matchmaking/players/add/{player}", player);
    }

    /// <summary>
    /// Add a group of players to the matchmaking queue
    /// </summary>
    /// <param name="groupModel">Group to add</param>
    /// <returns>Response</returns>
    public async Task<HttpResponseMessage> AddPlayers(PlayerGroupModel groupModel)
    {
        return await _client.PostAsJsonAsync(
            $"api/Matchmaking/players/addrange/{groupModel}", groupModel);
    }

    /// <summary>
    /// Get all ready matches 
    /// </summary>
    /// <returns></returns>
    public async Task<ReadyMatchesModel> GetReadyMatches()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"api/Matchmaking/matches/{2}");

        ReadyMatchesModel matches = null;

        Console.WriteLine($"GetStatus matches: {response.StatusCode}");
        if(response.IsSuccessStatusCode)
        {
            matches = await response.Content.ReadFromJsonAsync<ReadyMatchesModel>();
        }

        return matches;
    }
}