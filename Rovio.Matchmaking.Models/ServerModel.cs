using System.ComponentModel.DataAnnotations;

namespace Rovio.Matchmaking.Models;

public record ServerModel
{
    /// <summary>
    /// The Game Service Id
    /// </summary>
    [Required]
    public Guid GameServiceId { get; init; }
}