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
 
    /// <summary>
    /// The session this player has been assigned to
    /// </summary>
    public Session? Session { get; private set; }

    public bool IsSessionFound { get; private set; }
    
    public void AssignToSession(Session session)
    {
        IsSessionFound = true;
        Session = session;
    }
}