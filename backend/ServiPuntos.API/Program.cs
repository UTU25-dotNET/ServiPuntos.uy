using Microsoft.AspNetCore.Authentication.Cookies; // Para usar autenticaci�n basada en cookies
using Microsoft.AspNetCore.Authentication.Google; // Para usar autenticaci�n de Google
using Microsoft.AspNetCore.Authentication.JwtBearer; // Para usar autenticaci�n con tokens JWT
using Microsoft.IdentityModel.Tokens; // Para trabajar con elementos de seguridad de tokens
using System.Text; // Para operaciones de codificaci�n/decodificaci�n
using Microsoft.Extensions.Configuration; // Para acceder a configuraciones
using Microsoft.Extensions.DependencyInjection; // Para registro de servicios
using Microsoft.Extensions.Hosting; // Para configurar el entorno de hosting
using System.Security.Claims; // Para trabajar con claims de identidad
using Microsoft.OpenApi.Models; // Para configuraci�n de Swagger

// Creaci�n de la aplicaci�n web ASP.NET Core
var builder = WebApplication.CreateBuilder(args); // Inicializa el constructor de aplicaci�n web

// Agregar controladores a la aplicaci�n
builder.Services.AddControllers(); // Registra los servicios necesarios para los controladores API

// Agregar servicios de Swagger para documentaci�n de API
builder.Services.AddEndpointsApiExplorer(); // Agrega explorador de endpoints para API
builder.Services.AddSwaggerGen(); // Configura generaci�n de documentaci�n Swagger

// Configuraci�n JWT (JSON Web Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // Obtiene secci�n de configuraci�n para JWT
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]); // Convierte la clave secreta a bytes

// Configurar servicios de autenticaci�n
builder.Services.AddAuthentication(options =>
{
    // Configura los esquemas predeterminados de autenticaci�n
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Usa cookies como esquema predeterminado
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Usa Google como esquema de desaf�o
})
.AddCookie(options => // Configuraci�n de autenticaci�n por cookies
{
    options.LoginPath = "/api/auth/signin"; // Ruta para redirecci�n cuando se requiere autenticaci�n
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de expiraci�n de la cookie
})
.AddGoogle(options => // Configuraci�n de autenticaci�n con Google
{
    // Obtiene credenciales de Google desde la configuraci�n
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]; // ID de cliente de Google
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]; // Secreto de cliente de Google
    options.CallbackPath = "/api/auth/google-callback"; // Ruta de retorno despu�s de autenticaci�n en Google
    options.SaveTokens = true; // Guarda los tokens de acceso y actualizaci�n para uso posterior
})
.AddJwtBearer(options => // Configuraci�n de autenticaci�n con JWT
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Par�metros de validaci�n para tokens JWT
        ValidateIssuer = true, // Valida el emisor del token
        ValidateAudience = true, // Valida la audiencia del token
        ValidateLifetime = true, // Verifica que el token no haya expirado
        ValidateIssuerSigningKey = true, // Verifica la firma del token
        ValidIssuer = jwtSettings["Issuer"], // Emisor v�lido del token
        ValidAudience = jwtSettings["Audience"], // Audiencia v�lida del token
        IssuerSigningKey = new SymmetricSecurityKey(secretKey) // Clave para verificar la firma del token
    };
});

// Agregar el servicio JwtTokenService al contenedor de dependencias
builder.Services.AddScoped<JwtTokenService>(); // Registra el servicio con �mbito de solicitud (scoped)

// Construye la aplicaci�n web
var app = builder.Build(); // Finaliza la configuraci�n y construye la aplicaci�n

// Configurar el pipeline de solicitudes HTTP (middleware)
if (app.Environment.IsDevelopment()) // Verifica si estamos en entorno de desarrollo
{
    app.UseSwagger(); // Habilita middleware de Swagger para generar especificaci�n JSON
    app.UseSwaggerUI(); // Habilita la interfaz de usuario de Swagger
}

app.UseHttpsRedirection(); // Redirecciona las solicitudes HTTP a HTTPS
app.UseAuthentication(); // Habilita middleware de autenticaci�n
app.UseAuthorization(); // Habilita middleware de autorizaci�n
app.MapControllers(); // Mapea las rutas de los controladores

app.Run(); // Inicia la aplicaci�n web para escuchar solicitudes