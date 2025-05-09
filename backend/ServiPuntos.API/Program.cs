using Microsoft.EntityFrameworkCore;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Core.Interfaces;
var builder = WebApplication.CreateBuilder(args);

// 1. Registrar los DbContexts para multi-tenant
builder.Services.AddDbContext<TenantConfigurationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ServiPuntosDbContext>((sp, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrar el proveedor de tenant y HttpContextAccessor
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// 3. Registrar tus servicios y controladores
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddControllers();        // <- necesario para MapControllers()

var app = builder.Build();

// 4. Pipeline de middlewares
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware de resolución de tenant
app.UseMiddleware<ServiPuntos.API.Middleware.TenantResolutionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapear controladores y endpoints
app.MapControllers();

app.Run();

