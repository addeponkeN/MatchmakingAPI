using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class MatchmakingController : ControllerBase
{
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
    [HttpPost("players/add/{player}")]
    public async Task<ActionResult> AddPlayer(PlayerModel player)
    {
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
        foreach(var p in groupModel.Members)
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
        foreach(var p in groupModel.Members)
        {
            _mm.AddPlayer(p.ToMatchmakePlayer());
        }

        return Ok();
    }

    /// <summary>
    /// Get all matches that are ready to be started
    /// </summary>
    /// <param name="continent">The continent to get matches from</param>
    /// <returns>List of all ready matches</returns>
    [HttpGet("matches/{continent}")]
    public async Task<ActionResult<ReadyMatchesModel>> GetReadyMatches(Continents continent)
    {
        //  Pop all ready matches in the continent and convert to model
        var matches = _mm.PopReadyMatches(continent).ToModels();

        var model = new ReadyMatchesModel()
        {
            Matches = matches
        };

        Log.Debug($"Popped '{model.Matches.Count()}' matches");

        return Ok(model);
    }
    
    /// <summary>
    /// Adds a player to the matchmaking queue
    /// </summary>
    /// <param name="matchModel">Player to add</param>
    /// <returns>API result</returns>
    [HttpPost("players/add/{matchModel}")]
    public async Task<ActionResult> AddMissingPlayerMatch(MissingPlayerMatchModel matchModel)
    {
        if(!matchModel.Key.IsValid())
        {
            return Problem(title: "Invalid match id");
        }
        
        if(matchModel.Continent == Continents.None)
        {
            return Problem(title: "Invalid continent");
        }
        
        if(matchModel.MissingPlayersCount <= 0)
        {
            return Problem(title: "Invalid missing player count");
        }
        
        //  Get continent match container
        var container = _mm.GetContainer(matchModel.Continent);
        
        //  Create new match
        var match = _mm.CreateSession();
        
        container.AddMissingPlayerSession(match);

        return Ok();
    }
}