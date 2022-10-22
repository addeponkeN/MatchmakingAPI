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
 
    public bool IsMatchFound { get; private set; }
    
    /// <summary>
    /// The session this player has been assigned to
    /// </summary>
    public Session? Session { get; private set; }

    public void AssignToMatch(Session session)
    {
        IsMatchFound = true;
        Session = session;
    }
}