namespace Rovio.Matchmaking;

/// <summary>
/// Contains all matchmaking settings
/// </summary>
public class MatchmakingSettings
{
    internal MatchmakingManager manager;
    
    /// <summary>
    /// Minimum player count before the game can start
    /// </summary>
    public int MinPlayers { get; set; } = 2;
    
    /// <summary>
    /// Maximum player count
    /// </summary>
    public int MaxPlayer { get; set; } = 10;
    
    /// <summary>
    /// Minimum wait time before the game can start
    /// </summary>
    public float MinimumWaitTime { get; set; } = 6f;
    
}