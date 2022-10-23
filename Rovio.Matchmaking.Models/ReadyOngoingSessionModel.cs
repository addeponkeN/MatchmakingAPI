using System.Text.Json.Serialization;

namespace Rovio.Matchmaking.Models;

public record ReadyOngoingSessionModel
{
    [JsonPropertyName(PropertyAbbreviations.SessionCollection)]
    public IEnumerable<SessionModel> Sessions { get; init; }
}