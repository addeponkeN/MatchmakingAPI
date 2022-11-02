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
    private IdGenerator _idGenerator = new();

    private Dictionary<Continents, SessionContainer> _containers = new();
    private List<SessionContainer> _containerList = new();
    private Queue<MatchmakingPlayer> _players = new();
    private HashSet<Guid> _playersMarkedForRemoval = new();

    private object _queueLocker = new();

    public Matchmaker(Guid gameServiceId) : this(gameServiceId, new MatchmakingSettings())
    {
    }

    public Matchmaker(Guid gameServiceId, MatchmakingSettings settings)
    {
        GameServiceId = gameServiceId;
        Settings = settings;

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
    public void AddPlayer(MatchmakingPlayer player)
    {
        lock(_queueLocker)
        {
            _players.Enqueue(player);
        }
    }

    /// <summary>
    /// Remove a player from matchmaking by id
    /// </summary>
    /// <param name="playerId"></param>
    public void RemovePlayer(Guid playerId)
    {
        //  check if player already has been marked for removal
        if(_playersMarkedForRemoval.Contains(playerId))
        {
            return;
        }

        Log.Debug($"remove palyer: {playerId}");
            
        _playersMarkedForRemoval.Add(playerId);
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
    /// Get the next valid player in the queue
    /// </summary>
    /// <returns>First player in the queue</returns>
    private MatchmakingPlayer? GetNextPlayer()
    {
        MatchmakingPlayer? player = null;

        bool foundValidPlayer = false;

        while(!foundValidPlayer && _players.Count > 0)
        {
            player = _players.Dequeue();
            foundValidPlayer = !_playersMarkedForRemoval.Contains(player.UniqueId);
            if(!foundValidPlayer)
            {
                _playersMarkedForRemoval.Remove(player.UniqueId);
                Log.Debug($"getnextplayer - player marked {player.UniqueId}");
            }
        }

        return player;
    }

    internal bool IsPlayerMarkedForRemoval(Guid id)
    {
        return _playersMarkedForRemoval.Contains(id);
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
            MatchmakingPlayer? player;

            lock(_queueLocker)
            {
                // matchmake next player
                player = GetNextPlayer();
            }

            if(player == null)
                break;

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