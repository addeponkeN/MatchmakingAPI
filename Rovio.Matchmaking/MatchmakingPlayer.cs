using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// Matchmaking player
/// </summary>
public class MatchmakingPlayer
{
    internal MatchmakingPlayer() { }
    
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public Guid UniqueId { get; internal set; }

    /// <summary>
    /// The players preferred continent
    /// </summary>
    public Continents Continent { get; internal set; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    public int Rank { get; internal set; }

    public void Set(Guid id, Continents continent, int rank)
    {
        UniqueId = id;
        Continent = continent;
        Rank = rank;
    }
}