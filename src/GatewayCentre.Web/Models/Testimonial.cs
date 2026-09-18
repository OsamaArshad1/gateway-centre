using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

public class Testimonial
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string AuthorName { get; set; } = string.Empty;

    [StringLength(120)]
    public string? EventType { get; set; }

    [Required, StringLength(1000)]
    public string Quote { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; } = true;
}
