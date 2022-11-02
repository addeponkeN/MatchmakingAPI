namespace Rovio.Matchmaking;

public class MatchmakingManager
{
    private Dictionary<Guid, Matchmaker> _matchmakers = new();
    private List<Matchmaker> _matchmakersList = new();
    private Stack<MatchmakingPlayer> _pool = new();

    /// <summary>
    /// Add a matchmaker.
    /// Should be called when a game service is added.
    /// </summary>
    /// <param name="matchmaker">New matchmaker</param>
    public void Add(Matchmaker matchmaker)
    {
        matchmaker.Settings.manager = this;
        _matchmakersList.Add(matchmaker);
        _matchmakers.Add(matchmaker.GameServiceId, matchmaker);
    }
    
    /// <summary>
    /// Get 
    /// </summary>
    /// <param name="gameServiceId">Game service id</param>
    /// <param name="matchmaker">Matchmaker with the <paramref name="gameServiceId"/></param>
    /// <returns>A matchmaker matching the <paramref name="gameServiceId"/> from the collection</returns>
    public bool TryGetMatchmaker(Guid gameServiceId, out Matchmaker matchmaker)
    {
        return _matchmakers.TryGetValue(gameServiceId, out matchmaker);
    }

    /// <summary>
    /// Update all matchmakers.
    /// Should be called at least once a second.
    /// </summary>
    public void Update()
    {
        for(int i = 0; i < _matchmakersList.Count; i++)
        {
            _matchmakersList[i].Update();
        }
    }

    public MatchmakingPlayer CreatePlayer()
    {
        return _pool.Count > 0 ? _pool.Pop() : new();
    }

    public void ReturnPlayer(MatchmakingPlayer player)
    {
        _pool.Push(player);
    }
}