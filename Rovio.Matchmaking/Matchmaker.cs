using System.Diagnostics;
using Rovio.Utility;

namespace Rovio.Matchmaking;

public class Matchmaker
{
    public Guid GameServiceId { get; init; }
    public MatchmakingSettings Settings { get; }

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

    public SessionContainer GetContainer(Continents cont)
    {
        return _containers[cont];
    }

    public Session CreateSession()
    {
        return new Session(this) {Id = _idGenerator.GetId()};
    }

    public void AddPlayer(Player player)
    {
        lock(_queueLocker)
        {
            _players.Enqueue(player);
        }
    }

    public IEnumerable<Session> PopReadySessions(Continents continent)
    {
        var container = GetContainer(continent);
        return container.PopReadySessions();
    }

    public void Update()
    {
        UpdateMatchmaking();
        UpdateSessions();
    }

    public void UpdateSessions()
    {
        for(int i = 0; i < _containerList.Count; i++)
        {
            var container = _containerList[i];
            container.UpdateCurrentSession();
        }
    }

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

        for(int i = 0; i < length; i++)
        {
            Player player;

            lock(_queueLocker)
            {
                player = _players.Dequeue();
            }

            var container = GetContainer(player.Continent);
            var session = container.GetCurrentSession();

            session.AddPlayer(player);

            if(session.IsReady())
            {
                container.SetSessionReady(session);
                startedSessionsCount++;
            }
        }

        sw.Stop();
        Console.WriteLine(
            $"time: {sw.Elapsed.TotalMilliseconds}ms  |  '{startedSessionsCount}' sessions started  |  '{length}' players matched");
    }



}