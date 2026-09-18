using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

public enum GalleryCategory
{
    Weddings,
    Corporate,
    Catering,
    Halls,
    Other
}

public class GalleryImage
{
    public int Id { get; set; }

    [Required, StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Caption { get; set; }

    public GalleryCategory Category { get; set; } = GalleryCategory.Other;

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
