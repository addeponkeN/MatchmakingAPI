using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// Match session
/// </summary>
public class MatchmakingSession
{
    /// <summary>
    /// Session id/key
    /// </summary>
    public UniqueKey Id { get; init; }

    /// <summary>
    /// All the players in the session
    /// </summary>
    public List<Player> Players { get; }

    /// <summary>
    /// Location of the session
    /// </summary>
    public Continents Continent { get; set; }

    public bool IsStarted { get; private set; }

    public Guid OwnerToken { get; private set; }

    public int MissingPlayersCount { get; private set; }

    public bool IsOngoing => MissingPlayersCount > 0;

    private DateTime _previousTime;
    private float _activeTimer;
    private Matchmaker _mm;

    public MatchmakingSession(Matchmaker mm)
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

        if(Players.Count <= 0)
        {
            _previousTime = DateTime.Now;
        }

        Players.Add(player);
        return true;
    }

    public void SetAsOngoing(int missingPlayersCount, Guid serverToken)
    {
        MissingPlayersCount = missingPlayersCount;
        OwnerToken = serverToken;
    }

    /// <summary>
    /// Matchmaking is complete and is ready to start
    /// </summary>
    /// <returns>Session is ready to start</returns>
    public bool IsReady()
    {
        if(MissingPlayersCount > 0 && Players.Count >= MissingPlayersCount)
        {
            return true;
        }

        if(Players.Count >= _mm.Settings.MaxPlayer)
        {
            return true;
        }

        if(Players.Count >= _mm.Settings.MinPlayers && _activeTimer > _mm.Settings.MinimumWaitTime)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Start the session
    /// </summary>
    public void Start()
    {
        IsStarted = true;
    }

    /// <summary>
    /// </summary>
    /// <returns>Current player count is above the minimum player count (inclusive)</returns>
    public bool MinimumPlayerCountReached()
    {
        return Players.Count >= _mm.Settings.MinPlayers;
    }

    /// <summary>
    /// Check if the minimum wait time is met
    /// </summary>
    /// <returns>Minimum wait time is met</returns>
    public bool MinimumTimeWaited()
    {
        if(IsOngoing)
        {
            return false;
        }

        var timeNow = DateTime.Now;

        if(!MinimumPlayerCountReached())
        {
            _activeTimer = 0f;
            _previousTime = timeNow;
            return false;
        }

        var difference = timeNow.Subtract(_previousTime);

        _previousTime = timeNow;

        _activeTimer += (float)difference.TotalSeconds;

        return _activeTimer > _mm.Settings.MinimumWaitTime;
    }


    /// <summary>
    /// Reset fields and properties to default, keeping Id
    /// </summary>
    public MatchmakingSession Recycle()
    {
        IsStarted = false;

        _activeTimer = 0f;

        MissingPlayersCount = 0;
        OwnerToken = Guid.Empty;

        for(int i = 0; i < Players.Count; i++)
        {
            _mm.Settings.manager.ReturnPlayer(Players[i]);
        }

        Players.Clear();

        return this;
    }
}