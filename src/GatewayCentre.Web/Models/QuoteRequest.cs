using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

public enum QuoteStatus
{
    New,
    Contacted,
    Confirmed,
    Declined
}

public enum EventType
{
    Wedding,
    Engagement,
    Walima,
    CorporateEvent,
    Birthday,
    Other
}

public static class EventTypeExtensions
{
    public static string ToDisplayName(this EventType type) =>
        type == EventType.CorporateEvent ? "Corporate Event" : type.ToString();
}

public class QuoteRequest
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    public EventType EventType { get; set; } = EventType.Wedding;

    [Required]
    public DateOnly PreferredDate { get; set; }

    [Range(1, 5000)]
    public int GuestCount { get; set; }

    public int? HallId { get; set; }
    public Hall? Hall { get; set; }

    public int? PackageId { get; set; }
    public Package? Package { get; set; }

    [StringLength(2000)]
    public string? Message { get; set; }

    public QuoteStatus Status { get; set; } = QuoteStatus.New;

    [StringLength(2000)]
    public string? AdminNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
