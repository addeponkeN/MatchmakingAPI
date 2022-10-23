using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// Matchmaking player
/// </summary>
public class Player
{
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public UniqueKey Key { get; init; }

    /// <summary>
    /// The players preferred continent
    /// </summary>
    public Continents Continent { get; init; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    public int Rank { get; init; }
}