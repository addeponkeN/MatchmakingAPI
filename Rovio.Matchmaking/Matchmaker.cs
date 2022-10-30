using System.Diagnostics;
using Rovio.Utility;

namespace Rovio.Matchmaking;

public class Matchmaker
{
    /// <summary>
    /// The game service id
    /// </summary>
    public Guid GameServiceId { get; init; }

    /// <summary>
    /// Matchmaking settings
    /// </summary>
    public MatchmakingSettings Settings { get; }

    /// <summary>
    /// Generates id's to newly created sessions
    /// </summary>
    private IdGenerator _idGenerator;

    private Dictionary<Continents, SessionContainer> _containers;
    private List<SessionContainer> _containerList;
    private Queue<Player> _players;

    private object _queueLocker = new();

    public Matchmaker(Guid gameServiceId) : this(gameServiceId, new MatchmakingSettings())
    {
    }

    public Matchmaker(Guid gameServiceId, MatchmakingSettings settings)
    {
        GameServiceId = gameServiceId;
        Settings = settings;
        _players = new Queue<Player>();
        _idGenerator = new IdGenerator();
        _containers = new();
        _containerList = new();

        AddContainers();
    }

    /// <summary>
    /// Add a session container for each continent.
    /// </summary>
    private void AddContainers()
    {
        var continents = Enum.GetValues<Continents>();
        foreach(var continent in continents)
        {
            var container = new SessionContainer(this, continent);
            _containers.Add(continent, container);
            _containerList.Add(container);
        }
    }

    /// <summary>
    /// Get a container by continent.
    /// </summary>
    /// <param name="continent"></param>
    /// <returns>Session container by continent</returns>
    public SessionContainer GetContainer(Continents continent)
    {
        return _containers[continent];
    }

    /// <summary>
    /// Creates a new Session with a new id.
    /// </summary>
    /// <returns></returns>
    public MatchmakingSession CreateSession()
    {
        return new MatchmakingSession(this) {Id = _idGenerator.GetId()};
    }

    /// <summary>
    /// Add a player to the matchmaker
    /// </summary>
    /// <param name="player">Player to add</param>
    public void AddPlayer(Player player)
    {
        lock(_queueLocker)
        {
            _players.Enqueue(player);
        }
    }

    /// <summary>
    /// Return and remove all sessions that is ready start.
    /// </summary>
    /// <param name="continent"></param>
    /// <returns>Collection of all ready sessions</returns>
    public IEnumerable<MatchmakingSession> PopReadySessions(Continents continent)
    {
        var container = GetContainer(continent);
        return container.PopReadySessions();
    }

    /// <summary>
    /// Return and remove all sessions that is ready start.
    /// </summary>
    /// <param name="continent"></param>
    /// <param name="serverToken"></param>
    /// <returns>Collection of all ready sessions</returns>
    public IEnumerable<MatchmakingSession>? PopReadyOngoingSessions(Continents continent, Guid serverToken)
    {
        var container = GetContainer(continent);
        var sessions = container.PopReadyOngoingSessions(serverToken);
        return sessions;
    }

    /// <summary>
    /// Updates matchmaking and all containers current session
    /// </summary>
    public void Update()
    {
        UpdateMatchmaking();
        UpdateSessions();
    }

    /// <summary>
    /// Update all sessions
    /// </summary>
    public void UpdateSessions()
    {
        for(int i = 0; i < _containerList.Count; i++)
        {
            var container = _containerList[i];
            container.UpdateCurrentSession();
        }
    }

    /// <summary>
    /// Matches players to a session.
    /// Marks sessions as ready if the minimum start requirements are met.
    /// </summary>
    private void UpdateMatchmaking()
    {
        int playerCount;

        lock(_queueLocker)
        {
            playerCount = _players.Count;
        }

        //  no need to update if there are no players to matchmake with
        if(playerCount <= 0)
        {
            return;
        }

        //  max amount of players that can be matched together per update 
        const int bucketSize = 50_000;

        int length = Math.Min(bucketSize, playerCount);

        var sw = Stopwatch.StartNew();
        int startedSessionsCount = 0;
        int startedOngoingSessionsCount = 0;

        for(int i = 0; i < length; i++)
        {
            Player player;

            lock(_queueLocker)
            {
                // matchmake next player
                player = _players.Dequeue();
            }

            //  get session container by the player's preferred continent
            //  add the player to the current session.
            var container = GetContainer(player.Continent);
            var session = container.GetCurrentSession();
            session.AddPlayer(player);

            //  start the session if the minimum start requirements are met
            if(session.IsReady())
            {
                container.SetSessionReady(session);
                if(session.IsOngoing)
                    startedOngoingSessionsCount++;
                else
                    startedSessionsCount++;
            }
        }

        sw.Stop();
        Log.Debug(
            $"time: {sw.Elapsed.TotalMilliseconds}ms  |  '{startedSessionsCount}' sessions started  |  '{startedOngoingSessionsCount}' ongoings started  |  '{length}' players matched");
    }
}