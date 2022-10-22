using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// This contains info that is needed to add a match back into the matchmaker
/// and add the missing amount of players to the match.
/// High priority match. 
/// </summary>
public class MissingPlayerMatchModel
{
    /// <summary>
    /// Unique id/key of the match that is missing player(s)
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.UniqueKey)]
    public UniqueKey Key { get; set; }

    /// <summary>
    /// The location of the match
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.Continent)]
    public Continents Continent { get; set; }

    /// <summary>
    /// The amount of missing players from the game
    /// </summary>
    [Required]
    [JsonPropertyName(PropertyAbbreviations.MissingPlayersCount)]
    public int MissingPlayersCount { get; set; }
    
}