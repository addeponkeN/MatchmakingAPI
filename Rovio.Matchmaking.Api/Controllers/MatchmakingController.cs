using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Api.Services;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Controllers;

/*
 *  features
 *
 *      add player to queue
 *      match found
 *      player get queue position
 *      get total amount of players queuing
 *      get detailed amount of players queuing
 *          - total
 *          - region totals
 * 
 */

[ApiController]
[Route("api/[controller]")]
public class MatchmakingController : ControllerBase
{
    private PlayerRepository _repository;
    private Matchmaker _mm;
    
    public MatchmakingController(Matchmaker mm)
    {
        _mm = mm;
        _repository = new();
    }

    // [Route("api/[controller]/players")]
    // [Route("api/[controller]/players/{count}")]
    /// <summary>
    /// Get all players that are currently matchmaking
    /// </summary>
    /// <returns>All matchmaking players</returns>
    // [HttpGet]
    // public async Task AddPlayer(PlayerModel player)
    // {
    //     await _repository.AddPlayer(player);
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">Player info</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<MatchModel>> RequestMatchmake(PlayerModel player)
    {
        if(player == null)
        {
            return NotFound();
        }

        if(player.Id.IsInvalid())
        {
            return NotFound();
        }
        
        await Task.Run(() =>
        {
            return _mm.MatchmakePlayer(player.ToMatchmakePlayer());
        });

        return Problem("Could not find a match", string.Empty, 99);
    }


}