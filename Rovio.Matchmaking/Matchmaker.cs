namespace Rovio.Matchmaking;

public class Matchmaker
{
    public MatchmakingSettings Settings { get; }
    public List<Match> Matches { get; }

    private List<Match> _openMatches;
    private Queue<MatchmakePlayer> _players;

    private bool _isMatchmaking;

    public Matchmaker(MatchmakingSettings settings)
    {
        Settings = settings;
        Matches = new();
        _openMatches = new();
        _players = new();
    }

    private Match CreateMatch()
    {
        return new Match(this);
    }

    private Match GetOpenMatch()
    {
        return _openMatches.Count <= 0 ? CreateMatch() : _openMatches.First();
    }

    public void AddPlayer(MatchmakePlayer player)
    {
        _players.Enqueue(player);
    }

    public async Task<Match> MatchmakePlayer(MatchmakePlayer player)
    {
        AddPlayer(player);
        
        
        
        return null;
    }

    public async Task Matchmake()
    {
        if(_isMatchmaking)
            return;

        _isMatchmaking = true;
    }
}