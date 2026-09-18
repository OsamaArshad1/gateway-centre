using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

public class Hall
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int SeatingCapacity { get; set; }

    [StringLength(120)]
    public string? Floor { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; } = true;
}
