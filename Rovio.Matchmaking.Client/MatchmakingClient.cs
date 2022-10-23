using System.Net.Http.Headers;
using System.Net.Http.Json;
using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Client;

/// <summary>
/// Matchmaking API Client
/// </summary>
public class MatchmakingClient
{
    public ValidatedServerModel ValidatedServer
    {
        get => _validatedServer;
        set => _validatedServer = value;
    }

    public Continents Continent { get; init; }

    private bool IsValidated => _validatedServer != null;

    /// <summary>
    /// Http client
    /// </summary>
    private HttpClient _client;

    private ValidatedServerModel _validatedServer;
    private string _matchmakingRoute;
    private string _gameServiceRoute;

    public MatchmakingClient(Continents continent, string address, int port)
    {
        Continent = continent;

        _client = new()
        {
            BaseAddress = new Uri($"https://{address}:{port}"),
            Timeout = TimeSpan.FromSeconds(60 * 5),
        };

        _matchmakingRoute = "api/v1/Matchmaking/";
        _gameServiceRoute = "api/v1/GameServices/";

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="server"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> RegisterServer(ServerModel server)
    {
        if(IsValidated)
        {
            Log.Warning("Already validated");
            return null;
        }

        var response = await _client.PostAsJsonAsync(
            $"{_gameServiceRoute}register/{server}", server);

        if(response.IsSuccessStatusCode)
        {
            ValidatedServer = await response.Content.ReadFromJsonAsync<ValidatedServerModel>();
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
        if(!IsValidated)
        {
            Log.Warning("Not validated");
            return null;
        }

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
        if(!IsValidated)
        {
            Log.Warning("Not validated");
            return null;
        }

        return await _client.PostAsJsonAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/players/addrange/{groupModel}", groupModel);
    }

    /// <summary>
    /// Get all ready matches 
    /// </summary>
    /// <returns></returns>
    public async Task<ReadySessionsModel> GetReadySessions()
    {
        if(!IsValidated)
        {
            Log.Warning("Not validated");
            return null;
        }

        HttpResponseMessage response = await _client.GetAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/sessions/{Continent}");

        ReadySessionsModel sessions = null;

        if(response.IsSuccessStatusCode)
        {
            sessions = await response.Content.ReadFromJsonAsync<ReadySessionsModel>();
        }

        return sessions;
    }

    /// <summary>
    /// Get all ready ongoing matches 
    /// </summary>
    /// <returns></returns>
    public async Task<ReadyOngoingSessionModel> GetReadyOngoingSessions()
    {
        if(!IsValidated)
        {
            Log.Warning("Not validated");
            return null;
        }

        HttpResponseMessage response = await _client.GetAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/sessions/ongoing/{Continent}");

        ReadyOngoingSessionModel session = null;

        if(response.IsSuccessStatusCode)
        {
            session = await response.Content.ReadFromJsonAsync<ReadyOngoingSessionModel>();
        }

        return session;
    }


    /// <summary>
    /// Add an ongoing session with disconnected players back to the matchmaker.
    /// </summary>
    /// <param name="match">Ongoing session</param>
    /// <returns>Ongoing session response</returns>
    public async Task<HttpResponseMessage> AddOngoingMatch(OngoingSessionsModel match)
    {
        if(!IsValidated)
        {
            Log.Warning("Not validated");
            return null;
        }

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"{_matchmakingRoute}{_validatedServer.ServerId}/sessions/addongoing/{match}", match);

        // OngoingSessionResponseModel responseMatch = null;

        // if(response.IsSuccessStatusCode)
        // {
        // responseMatch = await response.Content.ReadFromJsonAsync<OngoingSessionResponseModel>();
        // }

        return response;
    }
}