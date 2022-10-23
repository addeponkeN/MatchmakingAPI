namespace Rovio.Matchmaking.Models;

/// <summary>
/// Contains data about game service
/// </summary>
public record GameService
{
    public Guid GameServiceId { get; init; }
    public string GameName { get; init; }

    public override string ToString()
    {
        return GameName;
    }
}