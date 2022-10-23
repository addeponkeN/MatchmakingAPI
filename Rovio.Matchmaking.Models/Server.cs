using System.ComponentModel.DataAnnotations;

namespace Rovio.Matchmaking.Models;

public record Server
{
    /// <summary>
    /// The Game Service Id
    /// </summary>
    [Required]
    public Guid GameServiceId { get; init; }
}