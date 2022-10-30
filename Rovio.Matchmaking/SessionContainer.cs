using Rovio.Utility;

namespace Rovio.Matchmaking;

/// <summary>
/// 
/// </summary>
public class SessionContainer
{
//  PROPERTIES
    public Continents Continent { get; }

//  MATCH COLLECTIONS
    private List<MatchmakingSession> _readySessions = new();
    private Stack<MatchmakingSession> _emptySessions = new();
    private Queue<MatchmakingSession> _ongoingSessions = new();
    private Dictionary<Guid, List<MatchmakingSession>> _readyOngoingSessions = new();

//  PRIVATE FIELDS
    private Matchmaker _mm;
    private MatchmakingSession _currentMatchmakingSession;
    private bool _isSetToClear;

    public SessionContainer(Matchmaker mm, Continents cont)
    {
        _mm = mm;
        Continent = cont;

        _currentMatchmakingSession = GetNewSession();
    }

    /// <summary>
    /// Get an empty inactive session
    /// </summary>
    /// <returns>Empty session</returns>
    public MatchmakingSession GetNewSession(bool ignoreOngoingSessions = false)
    {
        MatchmakingSession matchmakingSession;

        if(!ignoreOngoingSessions && _ongoingSessions.Count > 0)
        {
            matchmakingSession = _ongoingSessions.Dequeue();
            Log.Debug("New session is ongoing");
        }
        else
        {
            matchmakingSession = _emptySessions.Count <= 0 ? _mm.CreateSession() : _emptySessions.Pop();
        }

        return matchmakingSession;
    }

    /// <summary>
    /// Add a session that had a player disconnect mid-game
    /// This session will have higher priority in the matchmaker
    /// </summary>
    /// <param name="missingPlayersCount">The amount of players the session is missing</param>
    /// <param name="serverOwnerToken">The server who sent the request to add this session back to matchmaking</param>
    public void AddOngoingSession(int missingPlayersCount, Guid serverOwnerToken)
    {
        var session = GetNewSession(true);
        session.SetAsOngoing(missingPlayersCount, serverOwnerToken);
        _ongoingSessions.Enqueue(session);
        Log.Debug($"Added ongoing session - Missing: {missingPlayersCount},  token: {serverOwnerToken}");
    }

    /// <summary>
    /// Get the current session that is being used to match players
    /// </summary>
    /// <returns>Current session</returns>
    public MatchmakingSession GetCurrentSession()
    {
        return _currentMatchmakingSession.IsStarted
            ? _currentMatchmakingSession = GetNewSession()
            : _currentMatchmakingSession;
    }

    /// <summary>
    /// Pop all ready sessions and marks the container to recycle all sessions
    /// </summary>
    /// <returns>All ready sessions</returns>
    public IEnumerable<MatchmakingSession> PopReadySessions()
    {
        //  Clear all ready sessions if the container has already been marked to clear
        //  and return an empty collection
        if(_isSetToClear)
        {
            ClearReadySessions();
        }

        //  else mark to prepare clear and return all ready sessions
        else
        {
            _isSetToClear = true;
        }

        return _readySessions;
    }

    /// <summary>
    /// Pop all ready ongoing sessions and marks the container to recycle all sessions
    /// </summary>
    /// <returns>All ready sessions</returns>
    public IEnumerable<MatchmakingSession>? PopReadyOngoingSessions(Guid serverToken)
    {
        if(!_readyOngoingSessions.TryGetValue(serverToken, out var ongoingSessions))
        {
            return null;
        }

        _readyOngoingSessions.Remove(serverToken);

        return ongoingSessions;
    }

    /// <summary>
    /// Recycle all sessions that has been sent
    /// </summary>
    public void ClearReadySessions()
    {
        _isSetToClear = false;
        for(int i = 0; i < _readySessions.Count; i++)
        {
            _emptySessions.Push(_readySessions[i].Recycle());
        }

        _readySessions.Clear();
    }

    /// <summary>
    /// Mark a session as ready and add it to the ready sessions collection
    /// If the container has been marked to clear, clear & recycle all ready sessions and then add <param name="matchmakingSession"></param>
    /// </summary>
    /// <param name="matchmakingSession">Session to be marked as ready</param>
    public void SetSessionReady(MatchmakingSession matchmakingSession)
    {
        matchmakingSession.Start();

        // Add the session to the ready ongoing session collection if it is an ongoing session 
        if(matchmakingSession.IsOngoing)
        {
            if(!_readyOngoingSessions.TryGetValue(matchmakingSession.OwnerToken, out var list))
            {
                _readyOngoingSessions.Add(matchmakingSession.OwnerToken, list = new List<MatchmakingSession>());
            }

            list.Add(matchmakingSession);

            Log.Debug($"ongoing ready session done! - owner token: {matchmakingSession.OwnerToken}");
        }
        else
        {
            if(_isSetToClear)
            {
                ClearReadySessions();
            }

            _readySessions.Add(matchmakingSession);
        }
    }

    public void UpdateCurrentSession()
    {
        if(_currentMatchmakingSession.MinimumTimeWaited())
        {
            if(_currentMatchmakingSession.IsReady())
            {
                SetSessionReady(_currentMatchmakingSession);
                Log.Debug($"Minimum time waited - Starting session with '{_currentMatchmakingSession.Players.Count}' players");
                GetCurrentSession();
            }
        }
    }
}