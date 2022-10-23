using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Rovio.Utility;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// This contains info that is needed to add a session back into the matchmaker
/// and add the missing amount of players to the session.
/// High priority session. 
/// </summary>
public class OngoingSessions
{
    /// <summary>
    /// The location of the session
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