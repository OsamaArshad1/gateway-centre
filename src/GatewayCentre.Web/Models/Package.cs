using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

public class Package
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100_000_000)]
    public decimal StartingPriceRs { get; set; }

    /// <summary>One feature per line, rendered as a bullet list.</summary>
    [StringLength(4000)]
    public string FeaturesText { get; set; } = string.Empty;

    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; } = true;

    public IEnumerable<string> Features =>
        FeaturesText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
