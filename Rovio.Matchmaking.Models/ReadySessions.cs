using System.Text.Json.Serialization;

namespace Rovio.Matchmaking.Models;

public record ReadySessions
{
    [JsonPropertyName(PropertyAbbreviations.SessionCollection)]
    public IEnumerable<Session> Sessions { get; init; }
}