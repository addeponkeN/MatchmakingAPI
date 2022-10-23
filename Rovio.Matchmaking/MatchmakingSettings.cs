namespace Rovio.Matchmaking;

public class MatchmakingSettings
{
    public int MinPlayers { get; set; } = 2;
    public int MaxPlayer { get; set; } = 10;
    public float MinimumWaitTime { get; set; } = 6f;
}