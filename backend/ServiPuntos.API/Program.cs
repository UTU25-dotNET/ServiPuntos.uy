using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL; 
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiPuntos.Application.Services;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.Middleware;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Repositories;
using ServiPuntos.Infrastructure.Middleware;

using System.Text;
using System.Security.Claims;
using System.Text;

// Creaci�n de la aplicaci�n web ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);

// Agregar controladores a la aplicaci�n
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Agregar servicios de Swagger para documentaci�n de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n JWT (JSON Web Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKeyString = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKeyString))
{
    throw new InvalidOperationException("JWT SecretKey is not configured.");
}
var secretKey = Encoding.UTF8.GetBytes(secretKeyString);

//Soporte de sesion
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Para HTTPS
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configurar servicios de autenticaci�n
/*builder.Services.AddAuthentication(options =>
{
    // Para APIs REST, JWT Bearer debería ser el esquema por defecto
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    // Mantener Cookies para la parte web
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.Cookie.IsEssential = true;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
    
    // Configuración de eventos para debugging detallado
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine($"Token recibido: {(!string.IsNullOrEmpty(token) ? "SÍ" : "NO")}");
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"Primeros 20 caracteres: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
            if (context.Exception.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {context.Exception.InnerException.Message}");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"JWT Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"JWT Challenge initiated. Error: {context.Error}, Description: {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});*/

builder.Services.AddAuthentication(options =>
{
    // Cookies como esquema por defecto (para la parte WEB)
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // No establecer DefaultChallengeScheme - se manejará por controlador específico
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.Cookie.IsEssential = true;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

//Con esto permitimos solicitudes desde el frontend-web
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:3000") // Frontend HTTP
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // Importante para enviar cookies
            .SetIsOriginAllowed(_ => true));
});



<<<<<<< HEAD
=======

>>>>>>> origin/dev
builder.Services.AddDbContext<ServiPuntosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IConfigPlataformaRepository, ConfigPlataformaRepository>();
builder.Services.AddScoped<IConfigPlataformaService, ConfigPlataformaService>();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Registra los repositorios de NAFTA
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ICanjeRepository, CanjeRepository>();
builder.Services.AddScoped<IProductoCanjeableRepository, ProductoCanjeableRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IProductoUbicacionRepository, ProductoUbicacionRepository>();

// Registra los servicios de NAFTA
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IPuntosService, PuntosService>();
builder.Services.AddScoped<IProductoCanjeableService, ProductoCanjeableService>();
builder.Services.AddScoped<ICanjeService, CanjeService>();
builder.Services.AddScoped<IPointsRuleEngine, PointsRuleEngine>();
builder.Services.AddScoped<INAFTAService, NAFTAService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IProductoUbicacionService, ProductoUbicacionService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();

// Construye la aplicaci�n web
var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP (middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Importante para asegurar HTTPS
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseRouting();
app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<TenantMiddleware>();
app.MapControllers();


app.Run();