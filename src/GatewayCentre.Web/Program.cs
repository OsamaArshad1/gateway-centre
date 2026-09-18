using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using GatewayCentre.Web.Components;
using GatewayCentre.Web.Components.Account;
using GatewayCentre.Web.Data;
using GatewayCentre.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var uploadsPath = StoragePaths.ResolveUploadsPath(builder.Configuration, builder.Environment);
Directory.CreateDirectory(uploadsPath);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(DbSeeder.AdministratorRole));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var dbDirectory = Path.GetDirectoryName(new SqliteConnectionStringBuilder(connectionString).DataSource);
if (!string.IsNullOrEmpty(dbDirectory))
{
    Directory.CreateDirectory(Path.IsPathRooted(dbDirectory) ? dbDirectory : Path.Combine(builder.Environment.ContentRootPath, dbDirectory));
}
// Blazor Server components resolve their own short-lived context from the factory to avoid
// concurrent use of a single scoped DbContext across simultaneously-rendering components.
// The scoped ApplicationDbContext registration below (required by Identity) is served from the
// same factory rather than a separate AddDbContext call, which would otherwise register
// DbContextOptions twice under conflicting lifetimes.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddSingleton<SiteContentService>();
builder.Services.AddScoped<ImageUploadService>();

var app = builder.Build();

await DbSeeder.SeedAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Serves admin-uploaded images from their resolved storage path (which may live outside the
// deployed app folder in production — see StoragePaths), independent of the build-time static
// asset manifest that MapStaticAssets below handles for the app's own bundled wwwroot content.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
