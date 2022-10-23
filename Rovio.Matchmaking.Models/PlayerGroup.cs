using System.Text.Json.Serialization;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// A collection of player models
/// </summary>
public record PlayerGroup
{
    [JsonPropertyName(PropertyAbbreviations.PlayerModelCollection)]
    public IEnumerable<Player> Players { get; init; }
}