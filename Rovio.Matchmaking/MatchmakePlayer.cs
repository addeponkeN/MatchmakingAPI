using Rovio.Utility;

namespace Rovio.Matchmaking;

public class MatchmakePlayer
{
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public UniqueId Id { get; init; }

    /// <summary>
    /// Where the player is connecting from
    /// </summary>
    public Regions Region { get; init; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    public int Rank { get; init; }
 
    public bool IsMatchFound { get; set; }
    
}