namespace Rovio.Matchmaking.Models;

public record ValidatedServerModel
{
    public Guid ServerId { get; init; }
    public Guid GameServiceId { get; init; }
}