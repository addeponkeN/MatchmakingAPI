namespace Rovio.Matchmaking.Models;

/// <summary>
/// Contains abbreviations for model property names to reduce JSON string size
/// </summary>
public static class PropertyAbbreviations
{
    /// <summary>
    /// Key
    /// </summary>
    // public const string UniqueKey = "k";
    public const string UniqueKey = "UniqueKey";
    
    /// <summary>
    /// Name
    /// </summary>
    // public const string PlayerModelName = "n";
    public const string PlayerModelName = "Name";
    
    /// <summary>
    /// Continent
    /// </summary>
    // public const string Continent = "c";
    public const string Continent = "Continent";
    
    /// <summary>
    /// Rank
    /// </summary>
    // public const string PlayerModelRank = "r";
    public const string PlayerModelRank = "Rank";

    /// <summary>
    /// Players
    /// </summary>
    // public const string PlayerModelCollection = "p";
    public const string PlayerModelCollection = "Players";
    
    /// <summary>
    /// MissingPlayerCount
    /// </summary>
    // public const string MissingPlayersCount = "m";
    public const string MissingPlayersCount = "MissingPlayers";

    /// <summary>
    /// Sessions
    /// </summary>
    // public const string SessionCollection = "s";
    public const string SessionCollection = "Sessions";
}