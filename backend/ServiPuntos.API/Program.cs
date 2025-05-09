using Microsoft.EntityFrameworkCore;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Core.Interfaces;                // <- para ITenantProvider, ITenantResolver, ITenantContext
using ServiPuntos.API.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;                // <- para TenantResolutionMiddleware y TenantMiddleware

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar los DbContexts para multi-tenant
builder.Services.AddDbContext<TenantConfigurationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ServiPuntosDbContext>((sp, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrar el proveedor de tenant, resolver y tenant context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();    // <- traduce X-Tenant-Name a Guid
builder.Services.AddScoped<ITenantContext, TenantContext>();      // <- almacena el TenantId

// 3. Registrar tus servicios y controladores
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddControllers();

var app = builder.Build();

// 4. Pipeline de middlewares
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Extrae el nombre del subdominio/host y lo pone en X-Tenant-Name
app.UseMiddleware<TenantResolutionMiddleware>();

// Valida ese nombre, obtiene el Guid y lo guarda en tenantContext.TenantId
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapear controladores y endpoints
app.MapControllers();

app.Run();
