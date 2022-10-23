using Rovio.Utility;

namespace Rovio.Matchmaking;

public class Player
{
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public UniqueKey Key { get; init; }

    /// <summary>
    /// Where the player is connecting from
    /// </summary>
    public Continents Continent { get; init; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    public int Rank { get; init; }
}