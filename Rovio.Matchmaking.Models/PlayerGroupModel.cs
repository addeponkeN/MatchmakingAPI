using System.Text.Json.Serialization;

namespace Rovio.Matchmaking.Models;

/// <summary>
/// A collection of player models
/// </summary>
public record PlayerGroupModel
{
    [JsonPropertyName(PropertyAbbreviations.PlayerModelCollection)]
    public IEnumerable<PlayerModel> Players { get; init; }
}