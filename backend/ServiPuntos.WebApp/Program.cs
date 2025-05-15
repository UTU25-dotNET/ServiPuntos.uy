using System;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configuración de servicios
// -------------------------

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

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

// Multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Autenticación y Autorización con Cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";   // Ruta del login
        options.AccessDeniedPath = "/Account/AccessDenied";   // Ruta de acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración
    });

builder.Services.AddAuthorization();


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
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();
