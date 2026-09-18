using GatewayCentre.Web.Data;
using GatewayCentre.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GatewayCentre.Web.Services;

/// <summary>
/// Thin cache in front of the single-row SiteSettings table. Every public page reads settings
/// (nav/footer/contact/map), so we avoid a DB round trip per render and only refresh after an admin save.
/// </summary>
public class SiteContentService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    private SiteSettings? _cached;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<SiteSettings> GetSettingsAsync()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        await _lock.WaitAsync();
        try
        {
            if (_cached is null)
            {
                await using var db = await dbFactory.CreateDbContextAsync();
                _cached = await db.SiteSettings.AsNoTracking().FirstOrDefaultAsync() ?? new SiteSettings();
            }
        }
        finally
        {
            _lock.Release();
        }

        return _cached;
    }

    public void InvalidateCache() => _cached = null;
}
