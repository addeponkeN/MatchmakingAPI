using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api.Controllers;

/// <summary>
/// Handles the Matchmaking API calls.
/// A token is required to make any calls.
/// A token can be retrieved from the <see cref="GameServicesController"/>.
/// <remarks>Note to self: A token may not be required if used internally!</remarks>
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MatchmakingController : ControllerBase
{
    private MatchmakingManager _manager;
    private IClientRepository _serverRepository;

    public MatchmakingController(MatchmakingManager manager, IClientRepository serverPepository)
    {
        _manager = manager;
        _serverRepository = serverPepository;
    }

    /// <summary>
    /// Test
    /// </summary>
    /// <returns>API result</returns>
    [HttpGet("test")]
    public async Task<ActionResult<int>> GetTest()
    {
        return Ok(1337);
    }

    /// <summary>
    /// Adds a player to the matchmaking queue
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="player">Player to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/players/add/{player}")]
    public async Task<ActionResult> AddPlayer(Guid token, PlayerModel player)
    {
        if(!_serverRepository.TryGetGameServiceId(token, out Guid gameServiceId))
        {
            return Problem(title: "Invalid token");
        }
        
        if(player == null)
        {
            return Problem(title: "Invalid player info");
        }

        if(!player.Key.IsValid())
        {
            return Problem(title: "Invalid player id/key");
        }

        if(player.Continent == Continents.None)
        {
            return Problem(title: "Invalid player continent");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        matchmaker.AddPlayer(player.ToMatchmakePlayer());

        return Ok();
    }

    /// <summary>
    /// Adds players to the matchmaking queue
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="groupModel">Players to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/players/addrange/{groupModel}")]
    public async Task<ActionResult> AddPlayers(Guid token, PlayerGroupModel groupModel)
    {
        if(!_serverRepository.TryGetGameServiceId(token, out Guid gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(groupModel == null)
        {
            return Problem(title: "Invalid parameter");
        }

        //  validate all members
        foreach(var p in groupModel.Players)
        {
            if(!p.Key.IsValid())
            {
                return Problem(title: "Invalid player id/key");
            }

            if(p.Continent == Continents.None)
            {
                return Problem(title: "Invalid player continent");
            }
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  add members to matchmaking
        foreach(var p in groupModel.Players)
        {
            matchmaker.AddPlayer(p.ToMatchmakePlayer());
        }

        return Ok();
    }

    /// <summary>
    /// Get all sessions that are ready to be started
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="continent">The continent to get sessions from</param>
    /// <returns>List of all ready sessions</returns>
    [HttpGet("{token}/sessions/{continent}")]
    public async Task<ActionResult<ReadySessionsModel>> GetReadySessions(Guid token, Continents continent)
    {
        if(!_serverRepository.TryGetGameServiceId(token, out Guid gameServiceId))
        {
            return Problem(title: "Invalid token", statusCode: 101);
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  Pop all ready sessions in the continent and convert to model
        var sessions = matchmaker.PopReadySessions(continent).ToModels();

        var model = new ReadySessionsModel()
        {
            Sessions = sessions
        };

        Log.Debug($"Popped '{model.Sessions.Count()}' sessions");

        return Ok(model);
    }

    /// <summary>
    /// Add an OngoingSession to the matchmaker
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="sessionModel">Session to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/sessions/addmissing/{sessionModel}")]
    public async Task<ActionResult> AddOngoingSession(Guid token, OngoingSessionsModel sessionModel)
    {
        if(!_serverRepository.TryGetGameServiceId(token, out Guid gameServiceId))
        {
            return Problem(title: "Invalid token", statusCode: 101);
        }

        if(sessionModel.MissingPlayersCount <= 0)
        {
            return Problem(title: "Invalid missing player count");
        }
        
        if(!sessionModel.Key.IsValid())
        {
            return Problem(title: "Invalid session id");
        }

        if(sessionModel.Continent == Continents.None)
        {
            return Problem(title: "Invalid continent");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  Get continent session container
        var container = matchmaker.GetContainer(sessionModel.Continent);

        //  Create new session
        var session = container.GetNewSession();

        container.AddOngoingSession(session);

        return Ok();
    }
}