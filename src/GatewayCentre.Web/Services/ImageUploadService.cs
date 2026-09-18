using Microsoft.AspNetCore.Components.Forms;

namespace GatewayCentre.Web.Services;

public class ImageUploadService(IWebHostEnvironment env)
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    private const long MaxBytes = 8 * 1024 * 1024;

    /// <summary>Saves an uploaded image under wwwroot/uploads and returns its public relative URL.</summary>
    public async Task<string> SaveAsync(IBrowserFile file)
    {
        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Only JPG, PNG, WEBP or GIF images are allowed.");
        }

        var uploadsDir = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var extension = Path.GetExtension(file.Name) is { Length: > 0 } ext ? ext : ".jpg";
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = file.OpenReadStream(MaxBytes);
        await using var output = File.Create(fullPath);
        await stream.CopyToAsync(output);

        return $"/uploads/{fileName}";
    }
}
