using System.Diagnostics;
using Rovio.Utility;

namespace Rovio.Matchmaking;

public class Matchmaker
{
    public MatchmakingSettings Settings { get; }

    private IdGenerator _idGenerator;
    private Dictionary<Continents, SessionContainer> _containers;
    private Queue<Player> _players;

    private object _queueLocker = new();

    public Matchmaker() : this(new MatchmakingSettings())
    {
    }

    public Matchmaker(MatchmakingSettings settings)
    {
        Settings = settings;
        _players = new Queue<Player>();
        _idGenerator = new IdGenerator();
        _containers = new();

        AddContainers();
    }

    private void AddContainers()
    {
        var continents = Enum.GetValues<Continents>();
        foreach(var continent in continents)
        {
            _containers.Add(continent, new SessionContainer(this, continent));
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

    public async Task Update()
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

        //  max amount of players to match together per update 
        const int bucketSize = 50_000;

        int length = (int)MathF.Min(bucketSize, playerCount);

        //  debug
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
                session.Start();
                startedSessionsCount++;
            }
        }

        //  debug
        sw.Stop();
        Console.WriteLine(
            $"time: {sw.Elapsed.TotalMilliseconds}ms  |  '{startedSessionsCount}' sessions started  |  '{length}' players matched");
    }

    public IEnumerable<Session> PopReadySessions(Continents continent)
    {
        var container = GetContainer(continent);
        return container.PopReadySessions();
    }
}