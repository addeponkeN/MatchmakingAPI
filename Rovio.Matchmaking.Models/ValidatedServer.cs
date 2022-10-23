namespace Rovio.Matchmaking.Models;

public record ValidatedServer
{
    public Guid ServerId { get; init; }
    public Guid GameServiceId { get; init; }
}