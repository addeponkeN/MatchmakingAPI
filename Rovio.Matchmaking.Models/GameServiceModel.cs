namespace Rovio.Matchmaking.Models;

public record GameServiceModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public override string ToString()
    {
        return Name;
    }
}