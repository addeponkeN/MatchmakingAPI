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
    private List<Session> _readySessions;
    private Stack<Session> _emptySessions;
    private Queue<Session> _ongoingSessions;

//  PRIVATE FIELDS
    private Matchmaker _mm;
    private Session _currentSession;
    private bool _isSetToClear;

    public SessionContainer(Matchmaker mm, Continents cont)
    {
        _mm = mm;
        Continent = cont;

        _emptySessions = new Stack<Session>();
        _readySessions = new List<Session>();
        _ongoingSessions = new Queue<Session>();

        _currentSession = GetNewSession();
    }

    /// <summary>
    /// Get an empty inactive session
    /// </summary>
    /// <returns>Empty session</returns>
    public Session GetNewSession(bool ignoreOngoingSessions = false)
    {
        Session session;
        do
        {
            if(!ignoreOngoingSessions && _ongoingSessions.Count > 0)
            {
                session = _ongoingSessions.Dequeue();
            }
            else
            {
                session = _emptySessions.Count <= 0 ? _mm.CreateSession() : _emptySessions.Pop();
            }
            
        } while(session.IsActive);

        session.IsActive = true;

        return session;
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
    /// Add a session that had a player disconnect mid-game
    /// This session will have higher priority in the matchmaker
    /// </summary>
    /// <param name="session">Session that is missing (a) player(s)</param>
    public void AddOngoingSession(Session session)
    {
        _ongoingSessions.Enqueue(session);
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
        if(_isSetToClear)
        {
            ClearReadySessions();
        }

        session.Start();
        _readySessions.Add(session);
    }

    public void UpdateCurrentSession()
    {
        if(_currentSession.MinimumTimeWaited())
        {
            if(_currentSession.IsReady())
            {
                SetSessionReady(_currentSession);
                GetCurrentSession();
                Log.Debug("max time waited !! starting session");
            }
        }
    }
}