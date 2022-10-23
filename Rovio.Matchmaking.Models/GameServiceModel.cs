namespace Rovio.Matchmaking.Models;

/// <summary>
/// 
/// </summary>
public record GameServiceModel
{
    public Guid GameServiceId { get; init; }
    public string GameName { get; init; }

    public override string ToString()
    {
        return GameName;
    }
}