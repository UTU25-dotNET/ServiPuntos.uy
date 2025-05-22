using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configuración de servicios
// -------------------------

// MVC + API Controllers
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Para MapControllers()

// CORS para React
builder.Services.AddCors(o => o.AddPolicy("AllowReactApp", p =>
{
    p.WithOrigins("http://localhost:3000", "https://localhost:3000")
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials();
}));

// DB Context
builder.Services.AddDbContext<ServiPuntosDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ICanjeRepository, CanjeRepository>();
builder.Services.AddScoped<IProductoCanjeableRepository, ProductoCanjeableRepository>();
builder.Services.AddScoped<IProductoCanjeableService, ProductoCanjeableService>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IPuntosService, PuntosService>();
builder.Services.AddScoped<ICanjeService, CanjeService>();
builder.Services.AddScoped<IPointsRuleEngine, PointsRuleEngine>();
builder.Services.AddScoped<INAFTAService, NAFTAService>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();

// Multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// Authentication: JWT Bearer + Cookies
builder.Services.AddAuthentication(options =>
{
    // Priorizar JWT para API
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/AccountWApp/Login";
    options.AccessDeniedPath = "/AccountWApp/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminTenant", p => p.RequireRole("AdminTenant"));
});

var app = builder.Build();

// -------------------------
// Configuración de middlewares
// -------------------------

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS primero
app.UseCors("AllowReactApp");

// Autenticación (JWT y Cookies)
app.UseAuthentication();

// TenantMiddleware sólo fuera de /api/tenant
app.UseWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/api/tenant"),
    b => b.UseMiddleware<TenantMiddleware>()
);

app.UseAuthorization();

// -------------------------
// Mapeo de rutas
// -------------------------

// Mapear API controllers con atributos [Route]
app.MapControllers();

// Mapear MVC controllers + Views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
