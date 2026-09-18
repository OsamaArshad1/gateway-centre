namespace GatewayCentre.Web.Services;

/// <summary>
/// Resolves where uploaded images live on disk. Defaults to wwwroot/uploads for local dev.
/// On hosts where the deployed app folder is replaced on every deploy (e.g. Azure App Service),
/// set the Storage:UploadsPath configuration value (or STORAGE__UPLOADSPATH env var) to a path
/// outside the deployment folder — e.g. "D:\home\data\uploads" — so uploads survive redeploys.
/// </summary>
public static class StoragePaths
{
    public static string ResolveUploadsPath(IConfiguration configuration, IWebHostEnvironment env)
    {
        var configured = configuration["Storage:UploadsPath"];
        return string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(env.WebRootPath, "uploads")
            : configured;
    }
}
