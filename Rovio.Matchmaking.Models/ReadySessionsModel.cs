using System.Text.Json.Serialization;

namespace Rovio.Matchmaking.Models;

public record ReadySessionsModel
{
    [JsonPropertyName(PropertyAbbreviations.SessionCollection)]
    public IEnumerable<SessionModel> Sessions { get; init; }
}