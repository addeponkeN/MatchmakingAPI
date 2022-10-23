using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// Matchmaking player
/// </summary>
public class Player
{
    internal Player() { }
    
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public UniqueKey Key { get; internal set; }

    /// <summary>
    /// The players preferred continent
    /// </summary>
    public Continents Continent { get; internal set; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    public int Rank { get; internal set; }

    public void Set(UniqueKey key, Continents continent, int rank)
    {
        Key = key;
        Continent = continent;
        Rank = rank;
    }
}