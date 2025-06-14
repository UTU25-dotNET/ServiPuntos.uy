using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Application.Services;
using ServiPuntos.Application.Services.Rules;
using ServiPuntos.WebApp.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Set QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;
// -------------------------
// Configuración de servicios
// -------------------------

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ServiPuntos.WebApp.Filters.TenantNavbarFilter>();
});

// También registrar el filtro como servicio
builder.Services.AddScoped<ServiPuntos.WebApp.Filters.TenantNavbarFilter>();

// Contextos de base de datos
builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== REPOSITORIOS BÁSICOS =====
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();

// ===== REPOSITORIOS NAFTA (solo los que definitivamente existen) =====
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ICanjeRepository, CanjeRepository>();

// ===== REPOSITORIOS AUDIENCIA =====
builder.Services.AddScoped<IAudienciaRepository, AudienciaRepository>();

// ===== SERVICIOS BÁSICOS =====
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();

builder.Services.AddScoped<IPuntosService, PuntosService>();
builder.Services.AddScoped<IPointsRuleEngine, PointsRuleEngine>();

builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<ICanjeService, CanjeService>();

// ===== SERVICIOS AUDIENCIA =====
builder.Services.AddScoped<IAudienciaRuleEngine, AudienciaRuleEngine>();
builder.Services.AddScoped<IAudienciaService, AudienciaService>();
builder.Services.AddHostedService<ServiPuntos.WebApp.Services.AudienciaBackgroundService>();

// ===== SERVICIOS PRODUCTO CANJEABLE =====
builder.Services.AddScoped<IProductoCanjeableService, ProductoCanjeableService>();
builder.Services.AddScoped<IProductoCanjeableRepository, ProductoCanjeableRepository>();

// ===== SERVICIOS PRODUCTO UBICACION =====
builder.Services.AddScoped<IProductoUbicacionService, ProductoUbicacionService>();
builder.Services.AddScoped<IProductoUbicacionRepository, ProductoUbicacionRepository>();
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
builder.Services.AddScoped<IPromocionService, PromocionService>();

// ===== CONFIGURACIÓN PLATAFORMA =====
builder.Services.AddScoped<IConfigPlataformaRepository, ConfigPlataformaRepository>();
builder.Services.AddScoped<IConfigPlataformaService, ConfigPlataformaService>();



// ===== MULTI-TENANCY =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// ===== AUTENTICACIÓN =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AccountWApp/Login";
        options.AccessDeniedPath = "/AccountWApp/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminTenant", policy => policy.RequireRole("AdminTenant"));
});

// -------------------------
// Construcción de la app
// -------------------------

var app = builder.Build();

// -------------------------
// Configuración de middlewares
// -------------------------

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();

// -------------------------
// Ruteo
// -------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var defaultCulture = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new[] { defaultCulture },
    SupportedUICultures = new[] { defaultCulture }
};

CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
app.UseRequestLocalization(localizationOptions);

app.Run();