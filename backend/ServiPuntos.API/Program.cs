using Microsoft.AspNetCore.Authentication.Cookies; // Para usar autenticación basada en cookies
using Microsoft.AspNetCore.Authentication.Google; // Para usar autenticación de Google
using Microsoft.AspNetCore.Authentication.JwtBearer; // Para usar autenticación con tokens JWT
using Microsoft.IdentityModel.Tokens; // Para trabajar con elementos de seguridad de tokens
using System.Text; // Para operaciones de codificación/decodificación
using Microsoft.Extensions.Configuration; // Para acceder a configuraciones
using Microsoft.Extensions.DependencyInjection; // Para registro de servicios
using Microsoft.Extensions.Hosting; // Para configurar el entorno de hosting
using System.Security.Claims; // Para trabajar con claims de identidad
using Microsoft.OpenApi.Models; // Para configuración de Swagger

// Creación de la aplicación web ASP.NET Core
var builder = WebApplication.CreateBuilder(args); // Inicializa el constructor de aplicación web

// Agregar controladores a la aplicación
builder.Services.AddControllers(); // Registra los servicios necesarios para los controladores API

// Agregar servicios de Swagger para documentación de API
builder.Services.AddEndpointsApiExplorer(); // Agrega explorador de endpoints para API
builder.Services.AddSwaggerGen(); // Configura generación de documentación Swagger

// Configuración JWT (JSON Web Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // Obtiene sección de configuración para JWT
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]); // Convierte la clave secreta a bytes

// Configurar servicios de autenticación
builder.Services.AddAuthentication(options =>
{
    // Configura los esquemas predeterminados de autenticación
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Usa cookies como esquema predeterminado
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Usa Google como esquema de desafío
})
.AddCookie(options => // Configuración de autenticación por cookies
{
    options.LoginPath = "/api/auth/signin"; // Ruta para redirección cuando se requiere autenticación
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de expiración de la cookie
})
.AddGoogle(options => // Configuración de autenticación con Google
{
    // Obtiene credenciales de Google desde la configuración
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]; // ID de cliente de Google
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]; // Secreto de cliente de Google
    options.CallbackPath = "/api/auth/google-callback"; // Ruta de retorno después de autenticación en Google
    options.SaveTokens = true; // Guarda los tokens de acceso y actualización para uso posterior
})
.AddJwtBearer(options => // Configuración de autenticación con JWT
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Parámetros de validación para tokens JWT
        ValidateIssuer = true, // Valida el emisor del token
        ValidateAudience = true, // Valida la audiencia del token
        ValidateLifetime = true, // Verifica que el token no haya expirado
        ValidateIssuerSigningKey = true, // Verifica la firma del token
        ValidIssuer = jwtSettings["Issuer"], // Emisor válido del token
        ValidAudience = jwtSettings["Audience"], // Audiencia válida del token
        IssuerSigningKey = new SymmetricSecurityKey(secretKey) // Clave para verificar la firma del token
    };
});

// Agregar el servicio JwtTokenService al contenedor de dependencias
builder.Services.AddScoped<JwtTokenService>(); // Registra el servicio con ámbito de solicitud (scoped)

// Construye la aplicación web
var app = builder.Build(); // Finaliza la configuración y construye la aplicación

// Configurar el pipeline de solicitudes HTTP (middleware)
if (app.Environment.IsDevelopment()) // Verifica si estamos en entorno de desarrollo
{
    app.UseSwagger(); // Habilita middleware de Swagger para generar especificación JSON
    app.UseSwaggerUI(); // Habilita la interfaz de usuario de Swagger
}

app.UseHttpsRedirection(); // Redirecciona las solicitudes HTTP a HTTPS
app.UseAuthentication(); // Habilita middleware de autenticación
app.UseAuthorization(); // Habilita middleware de autorización
app.MapControllers(); // Mapea las rutas de los controladores

app.Run(); // Inicia la aplicación web para escuchar solicitudes