using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// Session model.
/// Each property has a JsonPropertyName abbreviation to reduce bandwidth usage.
/// </summary>
/// <para>Key = k</para>
/// <para>Players = p</para>
public record Session
{
    /// <summary>
    /// Unique id/key of the match
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.UniqueKey)]
    public UniqueKey Key { get; init; }
    
    /// <summary>
    /// Players that have been assigned to this match
    /// </summary>
    [JsonPropertyName(PropertyAbbreviations.PlayerCollection)]
    public IEnumerable<Player> Players { get; init; }
    
}
