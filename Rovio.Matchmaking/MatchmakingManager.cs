namespace Rovio.Matchmaking;

public class MatchmakingManager
{
    public Dictionary<Guid, Matchmaker> Matchmakers { get; init; }
    private List<Matchmaker> _matchmakersList;

    public MatchmakingManager()
    {
        Matchmakers = new();
        _matchmakersList = new();
    }

    public void Add(Matchmaker mm)
    {
        _matchmakersList.Add(mm);
        Matchmakers.Add(mm.GameServiceId, mm);
    }
    
    public bool TryGetMatchmaker(Guid id, out Matchmaker mm)
    {
        return Matchmakers.TryGetValue(id, out mm);
    }

    public void Update()
    {
        for(int i = 0; i < _matchmakersList.Count; i++)
        {
            _matchmakersList[i].Update();
        }
    }
}