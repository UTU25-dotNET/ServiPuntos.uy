using Microsoft.EntityFrameworkCore;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Infrastructure.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
//builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
//builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Configuramos la conexion a la base de datos
builder.Services.AddDbContext<TenantConfigurationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

// 4. Pipeline de middlewares
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware de resolución de tenant
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapear controladores y endpoints
app.MapControllers();

app.Run();

