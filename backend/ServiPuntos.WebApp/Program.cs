using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Application;
using ServiPuntos.Application.Services;


var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configuración de servicios
// -------------------------

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Contextos de base de datos
builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios y servicios de negocio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();


// Multi-tenancy
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Autenticación y Autorización con Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AccountWApp/Login";   // Ruta del login
        options.AccessDeniedPath = "/AccountWApp/AccessDenied";   // Ruta de acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración
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

// Configuración de Swagger (opcional, para desarrollo)
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();
app.UseStaticFiles();

// Asegurarse de que el enrutamiento esté configurado antes de autenticación
app.UseRouting();

// Middleware de Autenticación y Autorización

// 1. Autenticación
app.UseAuthentication();// Asegúrate de que se ejecute antes de TenantMiddleware

// 2. TenantMiddleware (después de autenticación, pero ANTES de MapControllerRoute)
app.UseMiddleware<TenantMiddleware>();

// 3. Autorización
app.UseAuthorization();

// -------------------------
// Ruteo
// -------------------------

// Configuración de la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
