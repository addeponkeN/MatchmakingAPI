using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MatchmakingController : ControllerBase
{
    private static string test = "players52";
    private Matchmaker _mm;

    public MatchmakingController()
    {
        _mm = Program.Matchmaker;
    }

    /// <summary>
    /// Test
    /// </summary>
    /// <returns>API result</returns>
    [HttpGet("test")]
    public async Task<ActionResult<int>> GetTest()
    {
        return Ok(5);
    }

    /// <summary>
    /// Adds a player to the matchmaking queue
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>API result</returns>
    [HttpPost("{game}/players/add/{player}")]
    public async Task<ActionResult> AddPlayer(Guid gameId, PlayerModel player)
    {
        Log.Debug($"GAME: {gameId}");
        if(player == null)
        {
            return Problem(title: "Invalid player info", statusCode: 101);
        }

        if(!player.Key.IsValid())
        {
            return Problem(title: "Invalid player id/key", statusCode: 102);
        }

        if(player.Continent == Continents.None)
        {
            return Problem(title: "Invalid player continent", statusCode: 103);
        }

        _mm.AddPlayer(player.ToMatchmakePlayer());

        return Ok();
    }

    /// <summary>
    /// Adds players to the matchmaking queue
    /// </summary>
    /// <param name="groupModel">Players to add</param>
    /// <returns>API response</returns>
    [HttpPost("players/addrange/{groupModel}")]
    public async Task<ActionResult> AddPlayers(PlayerGroupModel groupModel)
    {
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

        //  add members to matchmaking
        foreach(var p in groupModel.Players)
        {
            _mm.AddPlayer(p.ToMatchmakePlayer());
        }

        return Ok();
    }

    /// <summary>
    /// Get all sessions that are ready to be started
    /// </summary>
    /// <param name="continent">The continent to get sessions from</param>
    /// <returns>List of all ready sessions</returns>
    [HttpGet("sessions/{continent}")]
    public async Task<ActionResult<ReadySessionsModel>> GetReadySessions(Continents continent)
    {
        //  Pop all ready sessions in the continent and convert to model
        var sessions = _mm.PopReadySessions(continent).ToModels();

        var model = new ReadySessionsModel()
        {
            Sessions = sessions
        };

        Log.Debug($"Popped '{model.Sessions.Count()}' sessions");

        return Ok(model);
    }
    
    /// <summary>
    /// Adds a player to the matchmaking queue
    /// </summary>
    /// <param name="sessionModel">Player to add</param>
    /// <returns>API result</returns>
    [HttpPost("sessions/addmissing/{sessionModel}")]
    public async Task<ActionResult> AddMissingPlayerSession(MissingPlayerSessionModel sessionModel)
    {
        if(!sessionModel.Key.IsValid())
        {
            return Problem(title: "Invalid session id");
        }
        
        if(sessionModel.Continent == Continents.None)
        {
            return Problem(title: "Invalid continent");
        }
        
        if(sessionModel.MissingPlayersCount <= 0)
        {
            return Problem(title: "Invalid missing player count");
        }
        
        //  Get continent session container
        var container = _mm.GetContainer(sessionModel.Continent);
        
        //  Create new session
        var session = _mm.CreateSession();
        
        container.AddMissingPlayerSession(session);

        return Ok();
    }
}