using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Application.Services; // Ensure this is the correct namespace for UbicacionService
using System.Globalization;
using Microsoft.AspNetCore.Localization;


var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configuraci�n de servicios
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

//builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios y servicios de negocio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<ServiPuntos.WebApp.Filters.TenantNavbarFilter>();


builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

builder.Services.AddScoped<IUbicacionService, UbicacionService>();  
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();

// Multi-tenancy
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Autenticaci�n y Autorizaci�n con Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AccountWApp/Login";   // Ruta del login
        options.AccessDeniedPath = "/AccountWApp/AccessDenied";   // Ruta de acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminTenant", policy => policy.RequireRole("AdminTenant"));
});


// -------------------------
// Construcci�n de la app
// -------------------------

var app = builder.Build();

// -------------------------
// Configuraci�n de middlewares
// -------------------------

// Configuraci�n de Swagger (opcional, para desarrollo)
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();
app.UseStaticFiles();

// Asegurarse de que el enrutamiento est� configurado antes de autenticaci�n
app.UseRouting();

// Middleware de Autenticaci�n y Autorizaci�n

// 1. Autenticaci�n
app.UseAuthentication();// Aseg�rate de que se ejecute antes de TenantMiddleware

// 2. TenantMiddleware (despu�s de autenticaci�n, pero ANTES de MapControllerRoute)
app.UseMiddleware<TenantMiddleware>();

// 3. Autorizaci�n
app.UseAuthorization();

// -------------------------
// Ruteo
// -------------------------

// Configuraci�n de la ruta por defecto
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
