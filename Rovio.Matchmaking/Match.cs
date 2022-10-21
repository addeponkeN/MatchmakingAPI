namespace Rovio.Matchmaking;

/// <summary>
/// Match session
/// </summary>
public class Match
{
    /// <summary>
    /// All the players in the match
    /// </summary>
    public List<MatchmakePlayer> Players { get; }

    public bool IsStarted { get; private set; }
    
    private Matchmaker _mm;
    private int _matchPlayerCount;
    
    public Match(Matchmaker mm)
    {
        _mm = mm;
        Players = new();
    }

    /// <summary>
    /// Adds a player to the match
    /// </summary>
    /// <param name="player">Player model</param>
    /// <returns>Returns if adding was a success</returns>
    public bool AddPlayer(MatchmakePlayer player)
    {
        
        //  ensure the match cant exceed the max player limit
        if(Players.Count >= _mm.Settings.MaxPlayer)
        {
            return false;
        }
        
        Players.Add(player);
        return true;
    }

    /// <summary>
    /// Matchmaking is complete and is ready to start the match
    /// </summary>
    /// <returns>Match is ready to start</returns>
    public bool IsReady()
    {
        return Players.Count >= _mm.Settings.MaxPlayer;
    }

    /// <summary>
    /// Checks if a player left a match mid-game
    /// </summary>
    /// <returns></returns>
    public bool IsMissingPlayers()
    {
        return IsStarted && Players.Count < _matchPlayerCount;
    }
    
    /// <summary>
    /// Start match
    /// The match removed from matchmaking
    /// </summary>
    public void Start()
    {
        _matchPlayerCount = Players.Count;
        IsStarted = true;
    }
    
    public void Recycle()
    {
        _matchPlayerCount = 0;
        IsStarted = false;
        Players.Clear();
    }
    
}