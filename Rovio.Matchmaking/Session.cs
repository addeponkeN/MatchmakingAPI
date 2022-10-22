using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// Match session
/// </summary>
public class Session
{
    /// <summary>
    /// Session id/key
    /// </summary>
    public UniqueKey MatchId { get; init; }

    /// <summary>
    /// All the players in the match
    /// </summary>
    public List<Player> Players { get; }

    /// <summary>
    /// Location of the match
    /// </summary>
    public Continents Continent { get; set; }

    public bool IsStarted { get; private set; }

    private Matchmaker _mm;

    public Session(Matchmaker mm)
    {
        _mm = mm;
        Players = new();
    }

    /// <summary>
    /// Adds a player to the session
    /// </summary>
    /// <param name="player">Player model</param>
    /// <returns>Returns if adding was a success</returns>
    public bool AddPlayer(Player player)
    {
        //  ensure the session cant exceed the max player limit
        if(Players.Count >= _mm.Settings.MaxPlayer)
        {
            return false;
        }

        Players.Add(player);
        return true;
    }

    /// <summary>
    /// Matchmaking is complete and is ready to start the session
    /// </summary>
    /// <returns>Session is ready to start</returns>
    public bool IsReady()
    {
        return Players.Count >= _mm.Settings.MaxPlayer;
    }

    /// <summary>
    /// Start session and assign players to this session
    /// </summary>
    public void Start()
    {
        IsStarted = true;

        for(int i = 0; i < Players.Count; i++)
        {
            Players[i].AssignToMatch(this);
        }
    }

    /// <summary>
    /// Reset fields and properties to default, keeping Id
    /// </summary>
    public Session Recycle()
    {
        IsStarted = false;
        Players.Clear();
        return this;
    }
}