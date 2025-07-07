using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.IdentityModel.Tokens;
using ServiPuntos.Application.Services;
using ServiPuntos.Application.Services.Rules;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Infrastructure.MultiTenancy;
using ServiPuntos.Infrastructure.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text;
using System.IO;

// Creación de la aplicación web ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// 1) Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Trace);

// 2) MVC + JSON options
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4) JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKeyString = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKeyString))
    throw new InvalidOperationException("JWT SecretKey is not configured.");
var secretKey = Encoding.UTF8.GetBytes(secretKeyString);

// 5) Inicialización de Firebase Admin SDK
var firebasePath = builder.Configuration["Firebase:CredentialsPath"];
if (string.IsNullOrWhiteSpace(firebasePath))
    throw new InvalidOperationException("Firebase CredentialsPath no está configurado.");

var credentialsFullPath = Path.Combine(builder.Environment.ContentRootPath, firebasePath);

// *** LOG EN CONSOLA PARA VERIFICAR RUTA ***
Console.WriteLine($"[Firebase] Cargando credenciales desde: {credentialsFullPath}");

if (!File.Exists(credentialsFullPath))
    throw new FileNotFoundException($"No se encontró el archivo de credenciales de Firebase en '{credentialsFullPath}'");

var googleCred = GoogleCredential
    .FromFile(credentialsFullPath)
    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

FirebaseApp.Create(new AppOptions
{
    Credential = googleCred
});

// 6) Sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 7) Autenticación
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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

// 8) CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://servipuntosuy.up.railway.app", "https://servipuntosuy.up.railway.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true));
});

// 9) DbContext
builder.Services.AddDbContext<ServiPuntosDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 10) DI: servicios de aplicación
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
// 11) DI: multitenancy & repositorios
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IConfigPlataformaRepository, ConfigPlataformaRepository>();
builder.Services.AddScoped<IConfigPlataformaService, ConfigPlataformaService>();


// 12) DI: repositorios NAFTA
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ICanjeRepository, CanjeRepository>();
builder.Services.AddScoped<IProductoCanjeableRepository, ProductoCanjeableRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IProductoUbicacionRepository, ProductoUbicacionRepository>();
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IAudienciaRepository, AudienciaRepository>();

// 13) DI: servicios NAFTA
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IPuntosService, PuntosService>();
builder.Services.AddScoped<IProductoCanjeableService, ProductoCanjeableService>();
builder.Services.AddScoped<ICanjeService, CanjeService>();
builder.Services.AddScoped<IPointsRuleEngine, PointsRuleEngine>();
builder.Services.AddScoped<INAFTAService, NAFTAService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IProductoUbicacionService, ProductoUbicacionService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<IPromocionService, PromocionService>();
builder.Services.AddScoped<IFcmService, FcmService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IAudienciaRuleEngine, AudienciaRuleEngine>();
builder.Services.AddScoped<IAudienciaService, AudienciaService>();

// 14) Build & configure pipeline
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseRouting();
app.UseSession();
app.UseAuthentication();
// app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
