using System.ComponentModel;
using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// Player matchmaking model
/// </summary>
public record PlayerModel
{
    /// <summary>
    /// Unique id of the player
    /// </summary>
    public UniqueId Id { get; init; }
    
    /// <summary>
    /// Name of the player
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Where the player is connecting from
    /// </summary>
    public Regions Region { get; init; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    [DefaultValue(1)]
    public int Rank { get; init; }

}