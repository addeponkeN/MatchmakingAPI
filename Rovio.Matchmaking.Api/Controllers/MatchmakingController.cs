using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Api.Util;
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
[Route("api/v1/matchmaking")]
public class MatchmakingController : ControllerBase
{
    private MatchmakingManager _manager;
    private IClientRepository _serverRepository;

    public MatchmakingController(MatchmakingManager manager, IClientRepository serverPepository)
    {
        _manager = manager;
        _serverRepository = serverPepository;
    }

    private bool IsValidated(ref Guid token, out Guid gameServiceId)
    {
        return _serverRepository.TryGetGameServiceId(token, out gameServiceId);
    }

    /// <summary>
    /// Returns 1337
    /// </summary>
    /// <returns>API result</returns>
    [HttpGet("test")]
    public async Task<ActionResult<int>> GetTest()
    {
        await Task.Delay(100);
        return Ok(1337);
    }

    /// <summary>
    /// Add a player to the matchmaking queue
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="player">Player to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/players/add/{player}")]
    public ActionResult AddPlayer(Guid token, Player player)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(player == null)
        {
            return Problem(title: "Invalid player info");
        }

        if(player.UniqueId == Guid.Empty)
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

        matchmaker.AddPlayer(player.ToMatchmakePlayer(_manager));

        return Ok();
    }

    /// <summary>
    /// Add multiple players to the matchmaking queue
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="group">Players to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/players/addrange/{group}")]
    public ActionResult AddPlayers(Guid token, PlayerGroup group)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(group == null)
        {
            return Problem(title: "Invalid parameter");
        }

        //  validate all members
        foreach(var player in group.Players)
        {
            if(player.UniqueId == Guid.Empty)
            {
                return Problem(title: "Invalid player id/key");
            }

            if(player.Continent == Continents.None)
            {
                return Problem(title: "Invalid player continent");
            }
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  add members to matchmaking
        foreach(var p in group.Players)
        {
            matchmaker.AddPlayer(p.ToMatchmakePlayer(_manager));
        }

        return Ok();
    }

    /// <summary>
    /// Remove a player from matchmaking
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="playerId">Player id</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/players/remove/{playerId}")]
    public ActionResult RemovePlayer(Guid token, Guid playerId)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        matchmaker.RemovePlayer(playerId);

        return Ok();
    }

    /// <summary>
    /// Get all ready sessions.
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="continent">The continent to get sessions from</param>
    /// <returns>List of all ready sessions</returns>
    [HttpGet("{token}/sessions/{continent}")]
    public ActionResult<ReadySessions> GetReadySessions(Guid token, Continents continent)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  Pop all ready sessions in the continent and convert to model
        var sessions = matchmaker.PopReadySessions(continent).ToModels();

        var model = new ReadySessions()
        {
            Sessions = sessions
        };

        Log.Debug($"Popped '{model.Sessions.Count()}' sessions");

        return Ok(model);
    }

    /// <summary>
    /// Get all ongoing sessions. 
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="continent">The continent of the server</param>
    /// <returns>Collection of all ready ongoing sessions</returns>
    [HttpGet("{token}/sessions/ongoing/{continent}")]
    public ActionResult<ReadyOngoingSession> GetReadyOngoingSessions(Guid token, Continents continent)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        var readyOngoingCollection = matchmaker.PopReadyOngoingSessions(continent, token);

        if(readyOngoingCollection == null)
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  Pop all ready ongoing sessions in the continent that has been added by this token (server)
        var sessions = readyOngoingCollection.ToModels();

        var model = new ReadyOngoingSession()
        {
            Sessions = sessions
        };

        Log.Debug($"Popped '{model.Sessions.Count()}' sessions");

        return Ok(model);
    }

    /// <summary>
    /// Add an ongoing session to the matchmaker.
    /// </summary>
    /// <param name="token">Server token</param>
    /// <param name="session">Session to add</param>
    /// <returns>API result</returns>
    [HttpPost("{token}/sessions/addongoing/{session}")]
    public ActionResult AddOngoingSession(Guid token, OngoingSessions session)
    {
        if(!IsValidated(ref token, out var gameServiceId))
        {
            return Problem(title: "Invalid token");
        }

        if(session.MissingPlayersCount <= 0)
        {
            return Problem(title: "Invalid missing player count");
        }

        if(session.Continent == Continents.None)
        {
            return Problem(title: "Invalid continent");
        }

        if(!_manager.TryGetMatchmaker(gameServiceId, out var matchmaker))
        {
            return Problem(title: "Internal matchmaking error");
        }

        //  Get continent session container
        var container = matchmaker.GetContainer(session.Continent);

        //  Create new session
        container.AddOngoingSession(session.MissingPlayersCount, token);

        return Ok();
    }
}