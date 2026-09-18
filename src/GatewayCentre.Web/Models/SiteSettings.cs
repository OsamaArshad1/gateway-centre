using System.ComponentModel.DataAnnotations;

namespace GatewayCentre.Web.Models;

/// <summary>Single-row table of site-wide, admin-editable content (contact info, map, socials, hero text).</summary>
public class SiteSettings
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string SiteName { get; set; } = "Gateway Centre";

    [StringLength(200)]
    public string Tagline { get; set; } = "Marriage Hall & Guest House";

    [StringLength(200)]
    public string HeroHeadline { get; set; } = "Celebrate in Style at Gateway Centre";

    [StringLength(400)]
    public string HeroSubheadline { get; set; } = "The perfect venue for your weddings, parties, and corporate events.";

    [Required, Phone, StringLength(30)]
    public string PrimaryPhone { get; set; } = "+92 303 6890241";

    [Phone, StringLength(30)]
    public string? SecondaryPhone { get; set; } = "+92 320 1889473";

    [Phone, StringLength(30)]
    public string? WhatsAppNumber { get; set; } = "+923036890241";

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = "info@gatewaycentre.pk";

    [Required, StringLength(400)]
    public string Address { get; set; } = "The Gateway Centre, Central Public School Road, Khanpur";

    /// <summary>Latitude/longitude used to build the Google Maps embed and directions link.</summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [StringLength(300)]
    public string? FacebookUrl { get; set; }

    [StringLength(300)]
    public string? InstagramUrl { get; set; }

    [StringLength(300)]
    public string? TwitterUrl { get; set; }

    public string GoogleMapsEmbedUrl =>
        Latitude.HasValue && Longitude.HasValue
            ? $"https://maps.google.com/maps?q={Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}&z=15&output=embed"
            : $"https://maps.google.com/maps?q={Uri.EscapeDataString(Address)}&z=15&output=embed";

    public string GoogleMapsDirectionsUrl =>
        Latitude.HasValue && Longitude.HasValue
            ? $"https://www.google.com/maps/dir/?api=1&destination={Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
            : $"https://www.google.com/maps/dir/?api=1&destination={Uri.EscapeDataString(Address)}";
}
