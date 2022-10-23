using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// Player matchmaking model.
/// Each property has a JsonPropertyName abbreviation to reduce bandwidth usage.
/// </summary>
/// <para>Key = k</para>
/// <para>Name = n</para>
/// <para>Continent = c</para>
/// <para>Rank = r</para>
public record PlayerModel
{
    /// <summary>
    /// Unique id/key of the player
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.UniqueKey)]
    public UniqueKey Key { get; init; }
    
    /// <summary>
    /// The preferred continent of the player
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.Continent)]
    public Continents Continent { get; init; }
    
    /// <summary>
    /// The rank or skill level of the player
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.PlayerModelRank)]
    [DefaultValue(1)]
    public int Rank { get; init; }

}