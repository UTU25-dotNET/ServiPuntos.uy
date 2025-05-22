using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configuración de servicios
// -------------------------

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Contexto de base de datos
builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios y servicios de negocio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// **Registro de Ubicaciones**
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();

// Multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Autenticación y autorización con Cookies
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

// 1. Autenticación
app.UseAuthentication();

// 2. TenantMiddleware
app.UseMiddleware<TenantMiddleware>();

// 3. Autorización
app.UseAuthorization();

// -------------------------
// Ruteo
// -------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
