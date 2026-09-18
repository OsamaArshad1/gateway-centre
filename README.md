# Gateway Centre — Website

A modern, dynamic rebuild of the Gateway Centre marriage hall & guest house website
(Khanpur), built with **ASP.NET Core (.NET 10 LTS)**, **Blazor Web App** (server-rendered,
interactive where it matters), **EF Core + SQLite**, and **ASP.NET Core Identity** for the
admin login.

## What's included

- **Public site**: Home, Services, Our Spaces, Packages, Gallery (with filtering + lightbox),
  About (with FAQ), Location & Contact (Google Maps embed), and a **Request a Quote** form.
- **Dynamic quote requests**: every submission is saved to the database and visible in the
  admin dashboard, with a simple already-booked warning based on admin-managed blocked dates.
- **Admin dashboard** (`/admin`, login-protected): manage Halls, Packages, Gallery photos
  (with image upload), Testimonials, Quote Requests (status + notes), and Site Settings
  (contact info, hero text, social links, and the exact Google Maps pin).
- **Green/white theme** derived from the Gateway Centre logo, with light/dark mode support,
  a floating WhatsApp button, and a fully responsive layout.
- No external services required to run locally — SQLite database, no-key Google Maps embed,
  no paid APIs.

## Getting started

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
cd src/GatewayCentre.Web
dotnet run
```

The app creates and migrates its SQLite database automatically on first run
(`src/GatewayCentre.Web/App_Data/gatewaycentre.db`) and seeds:

- Starter content: the two halls, three sample packages, and two testimonials (edit or
  replace all of this from the admin dashboard).
- One admin account, email from `appsettings.json` → `SeedAdmin:Email` (defaults to
  `admin@gatewaycentre.pk`). No password is committed to the repo — on first run, if
  `SeedAdmin:Password` isn't configured, the app generates a random one and prints it once to
  the console, e.g.:

  ```
  ==================================================================
   First run: an admin account was created with a generated password.
     Email:    admin@gatewaycentre.pk
     Password: <random-generated>
   Log in at /Account/Login and change this password immediately.
  ==================================================================
  ```

  Copy that password from the terminal, log in, and change it right away from the account's
  Manage page. To set a known password instead (e.g. for CI or a fresh deploy), set the
  `SeedAdmin__Password` environment variable before first run — never commit a real password
  to `appsettings.json`.

## Setting the exact map location

Go to `/admin/settings` → **Location & Map**. The easiest way to get accurate coordinates:

1. Open [Google Maps](https://maps.google.com) and find the venue.
2. Right-click the exact spot → the coordinates appear at the top of the context menu; click
   to copy them.
3. Paste the latitude and longitude into the two fields in Site Settings and save.

The public Location page and the "Get Directions" link both update immediately (no API key
needed — this uses Google's free Maps Embed URL format).

## Project structure

```
src/GatewayCentre.Web/
  Components/
    Pages/          public pages (Home, Services, Gallery, RequestQuote, ...)
    Pages/Admin/     admin dashboard pages
    Layout/          SiteHeader, SiteFooter, AdminLayout, WhatsApp button
    Account/         scaffolded ASP.NET Core Identity pages (login, manage account, ...)
  Models/            EF Core entities (Hall, Package, GalleryImage, QuoteRequest, ...)
  Data/              ApplicationDbContext, DbSeeder, EF Core migrations
  Services/          SiteContentService (cached settings), ImageUploadService
  wwwroot/app.css    the whole design system (colors, components, layout)
```

## Deployment

The app is host-agnostic — it's a standard ASP.NET Core app with a file-based SQLite database,
so it runs on:

- **Azure App Service** (Linux or Windows) — publish with `dotnet publish` and deploy the
  output, or connect the repo for CI/CD. Make sure `App_Data` and `wwwroot/uploads` are on
  persistent storage (Azure App Service's local storage is fine for a single instance).
- **Windows shared hosting with IIS** — install the ASP.NET Core Hosting Bundle on the server,
  publish with `dotnet publish -c Release`, and point an IIS site at the output folder.
- **Any Linux VM** — publish, run behind `nginx`/Kestrel as a systemd service.

Before deploying:

1. Either let the app generate the first-run admin password and grab it from the deploy logs,
   or set a known one via the `SeedAdmin__Password` environment variable beforehand.
2. Log in and change the admin password immediately either way.
3. Confirm `App_Data/` and `wwwroot/uploads/` are writable by the app process and are on
   persistent (not ephemeral) storage.
4. If you outgrow SQLite (heavy concurrent traffic), swap the EF Core provider to SQL Server
   or PostgreSQL — the app's data access is entirely through EF Core, so this is a
   `Program.cs` + connection string change plus a fresh migration.

## Notes / things to follow up on

- The seeded map coordinates are an approximate Khanpur location — set the exact pin from
  `/admin/settings` (see above) before going live.
- All venue photos are placeholders until real photos are uploaded via the admin Halls and
  Gallery pages.
- The quote form has basic spam protection (a honeypot field); add Google reCAPTCHA if spam
  becomes an issue.
- Public self-registration is disabled — new admin/staff accounts must be created by an
  existing admin from `/Account/Register` while logged in.
