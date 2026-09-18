using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

/// <summary>A date (optionally scoped to one hall) that is already booked/unavailable, shown on the public availability calendar.</summary>
public class BlockedDate
{
    public int Id { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    public int? HallId { get; set; }
    public Hall? Hall { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }
}
