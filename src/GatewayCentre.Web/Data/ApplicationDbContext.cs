using GatewayCentre.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GatewayCentre.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
    public DbSet<BlockedDate> BlockedDates => Set<BlockedDate>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<QuoteRequest>()
            .HasOne(q => q.Hall)
            .WithMany()
            .HasForeignKey(q => q.HallId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<QuoteRequest>()
            .HasOne(q => q.Package)
            .WithMany()
            .HasForeignKey(q => q.PackageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<BlockedDate>()
            .HasOne(b => b.Hall)
            .WithMany()
            .HasForeignKey(b => b.HallId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BlockedDate>()
            .HasIndex(b => new { b.Date, b.HallId });
    }
}
