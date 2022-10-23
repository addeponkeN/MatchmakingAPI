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

    /// <summary>
    /// True if this session is the current session the matchmaker is using to match players.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if(_isActive)
            {
                _previousTime = DateTime.Now;
            }
        }
    }

    private bool _isActive;
    private DateTime _previousTime;
    private float _activeTimer;
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
    /// Matchmaking is complete and is ready to start
    /// </summary>
    /// <returns>Session is ready to start</returns>
    public bool IsReady()
    {
        if(Players.Count >= _mm.Settings.MaxPlayer)
            return true;

        if(Players.Count >= _mm.Settings.MinPlayers && _activeTimer > _mm.Settings.MinimumWaitTime)
            return true;

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
    /// Reset fields and properties to default, keeping Id
    /// </summary>
    public Session Recycle()
    {
        IsStarted = false;
        IsActive = false;
        _activeTimer = 0f;
        Players.Clear();
        return this;
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
}