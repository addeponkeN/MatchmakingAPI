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
    private List<Session> _readySessions = new();
    private Stack<Session> _emptySessions = new();
    private Queue<Session> _ongoingSessions = new();
    private Dictionary<Guid, List<Session>> _readyOngoingSessions = new();

//  PRIVATE FIELDS
    private Matchmaker _mm;
    private Session _currentSession;
    private bool _isSetToClear;

    public SessionContainer(Matchmaker mm, Continents cont)
    {
        _mm = mm;
        Continent = cont;

        _currentSession = GetNewSession();
    }

    /// <summary>
    /// Get an empty inactive session
    /// </summary>
    /// <returns>Empty session</returns>
    public Session GetNewSession(bool ignoreOngoingSessions = false)
    {
        Session session;

        if(!ignoreOngoingSessions && _ongoingSessions.Count > 0)
        {
            session = _ongoingSessions.Dequeue();
            Log.Debug("New session is ongoing");
        }
        else
        {
            session = _emptySessions.Count <= 0 ? _mm.CreateSession() : _emptySessions.Pop();
        }

        return session;
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
    public Session GetCurrentSession()
    {
        return _currentSession.IsStarted
            ? _currentSession = GetNewSession()
            : _currentSession;
    }

    /// <summary>
    /// Pop all ready sessions and marks the container to recycle all sessions
    /// </summary>
    /// <returns>All ready sessions</returns>
    public IEnumerable<Session> PopReadySessions()
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
    public IEnumerable<Session> PopReadyOngoingSessions(Guid serverToken)
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
    /// If the container has been marked to clear, clear & recycle all ready sessions and then add <param name="session"></param>
    /// </summary>
    /// <param name="session">Session to be marked as ready</param>
    public void SetSessionReady(Session session)
    {
        session.Start();

        // Add the session to the ready ongoing session collection if it is an ongoing session 
        if(session.IsOngoing)
        {
            if(!_readyOngoingSessions.TryGetValue(session.OwnerToken, out var list))
            {
                _readyOngoingSessions.Add(session.OwnerToken, list = new List<Session>());
            }

            list.Add(session);

            Log.Debug($"ongoing ready session done! - owner token: {session.OwnerToken}");
        }
        else
        {
            if(_isSetToClear)
            {
                ClearReadySessions();
            }

            _readySessions.Add(session);
        }
    }

    public void UpdateCurrentSession()
    {
        if(_currentSession.MinimumTimeWaited())
        {
            if(_currentSession.IsReady())
            {
                SetSessionReady(_currentSession);
                Log.Debug($"Minimum time waited - Starting session with '{_currentSession.Players.Count}' players");
                GetCurrentSession();
            }
        }
    }
}