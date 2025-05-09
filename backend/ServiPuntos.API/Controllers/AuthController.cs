using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controlador que maneja la autenticación con Google y la gestión de tokens JWT
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Servicio para la generación y gestión de tokens JWT
    private readonly JwtTokenService _jwtTokenService;

    /// <summary>
    /// Constructor que inyecta las dependencias necesarias
    /// </summary>
    /// <param name="jwtTokenService">Servicio para generar tokens JWT</param>
    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Inicia el flujo de autenticación con Google
    /// </summary>
    /// <remarks>
    /// Frontend: Redirecciona al usuario a esta URL para iniciar el proceso de login con Google
    /// Ejemplo: window.location.href = 'api/auth/signin';
    /// </remarks>
    /// <returns>Challenge de autenticación que redirige al usuario a Google</returns>
    [HttpGet("signin")]
    public IActionResult SignIn()
    {
        // Configuración de propiedades para la autenticación
        var properties = new AuthenticationProperties
        {
            // URL a la que Google redirigirá después de la autenticación exitosa
            RedirectUri = Url.Action(nameof(GoogleCallback)),
            // Información adicional para el proceso de autenticación
            Items =
            {
                // URL a la que redirigir después de procesar el callback
                { "returnUrl", Url.Action("Index", "Home") } //Diego: Configurar para que redirija a la pagina principal del Usuario
            }
        };

        // Inicia el desafío de autenticación con Google
        // Esto redireccionará automáticamente al usuario a la página de login de Google
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Maneja el callback de Google después de la autenticación
    /// </summary>
    /// <remarks>
    /// Diego: Esto noo se llama directamente. Google redirecciona para aca automáticamente.
    /// Después de procesar, en el frontend vas a recibir un token JWT que tenes que almacenar.
    /// Ejemplo: localStorage.setItem('token', response.token);
    /// </remarks>
    /// <returns>Un token JWT si la autenticación fue exitosa, o 401 Unauthorized si falló</returns>
    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        // Intenta autenticar al usuario usando las cookies temporales
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Si la autenticación falló, devuelve error 401
        if (!authenticateResult.Succeeded)
            return Unauthorized();

        // Extraer claims relevantes del usuario de Google para incluir en el JWT
        var claims = new List<Claim>();
        var userIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
        var emailClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
        var nameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Name);

        // Agrega los claims al token JWT usando nombres estandarizados
        if (userIdClaim != null) claims.Add(new Claim("sub", userIdClaim.Value)); // 'sub' es un identificador estándar para el usuario
        if (emailClaim != null) claims.Add(new Claim("email", emailClaim.Value)); // Email del usuario
        if (nameClaim != null) claims.Add(new Claim("name", nameClaim.Value));    // Nombre completo del usuario

        // Agregar rol predeterminado (esto podría venir de una base de datos en una aplicación real)
        claims.Add(new Claim("role", "user"));

        // Generar el token JWT a partir de los claims del usuario
        var token = _jwtTokenService.GenerateJwtToken(claims);

        // Devuelve el token JWT al cliente para que lo almacene y use en futuras peticiones
        return Ok(new { token });
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado mediante cookies de sesión
    /// </summary>
    /// <remarks>
    /// Frontend: Útil inmediatamente después del login para verificar los datos del usuario
    /// Ejemplo: fetch('api/auth/session-userinfo').then(res => res.json())
    /// </remarks>
    /// <returns>Información de claims del usuario o error 401 si no está autenticado</returns>
    [HttpGet("session-userinfo")]
    public async Task<IActionResult> GetSessionUserInfo()
    {
        // Intenta autenticar al usuario actual usando el esquema de cookies
        // Esto es útil justo después del callback de Google, antes de que el cliente use el JWT
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return Unauthorized();

        // Extrae y formatea los claims para devolverlos al cliente
        var userClaims = authenticateResult.Principal.Claims.Select(c => new { c.Type, c.Value });
        return Ok(userClaims);
    }

    /// <summary>
    /// Obtiene información del usuario a partir del token JWT
    /// </summary>
    /// <remarks>
    /// Diego: Llamar incluyendo el token JWT en el header de autorización
    /// Ejemplo: 
    /// fetch('api/auth/userinfo', {
    ///   headers: {
    ///     'Authorization': 'Bearer ' + localStorage.getItem('token')
    ///   }
    /// }).then(res => res.json())
    /// </remarks>
    /// <returns>Claims del usuario contenidos en el JWT</returns>
    [HttpGet("userinfo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Requiere token JWT válido
    public IActionResult GetUserInfo()
    {
        // Extrae los claims del usuario desde el token JWT proporcionado en el header
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(userClaims);
    }

    /// <summary>
    /// Cierra la sesión del usuario eliminando las cookies de autenticación
    /// </summary>
    /// <remarks>
    /// Diego: Llamar a esta API y luego eliminar el token JWT almacenado localmente
    /// Ejemplo:
    /// fetch('api/auth/logout').then(() => {
    ///   localStorage.removeItem('token');
    ///   window.location.href = '/login';
    /// });
    /// </remarks>
    /// <returns>Redirección a la página principal</returns>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        // Elimina las cookies de autenticación del servidor
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirige al cliente a la página principal
        // Nota: El frontend debe encargarse de eliminar el token JWT del almacenamiento local
        return Redirect("/"); // Redirige a la página principal, configurar para que rediriga al login
    }
}
