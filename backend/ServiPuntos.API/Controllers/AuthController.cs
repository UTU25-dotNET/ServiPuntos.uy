using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ServiPuntos.API.Data;

/// <summary>
/// Controlador que maneja la autenticación con Google y la gestión de tokens JWT
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Servicio para la generación y gestión de tokens JWT
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ApplicationDbContext _context;
    /// <summary>
    /// Constructor que inyecta las dependencias necesarias
    /// </summary>
    /// <param name="jwtTokenService">Servicio para generar tokens JWT</param>
    /// <summary>
    /// Constructor que inyecta las dependencias necesarias
    /// </summary>
    /// <param name="jwtTokenService">Servicio para generar tokens JWT</param>
    public AuthController(JwtTokenService jwtTokenService, IConfiguration configuration, IHttpClientFactory httpClientFactory, ApplicationDbContext context)
    {
        _context = context;
        // Inyectar el servicio de generación de tokens JWT

        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        // Generar un estado aleatorio
        var state = Guid.NewGuid().ToString();

        // Guardar el estado en una variable de sesión en lugar de una cookie
        try
        {
            // Intentar usar la sesión
            HttpContext.Session.SetString("GoogleOAuthState", state);
            Console.WriteLine($"[GoogleAuth] Estado guardado en sesión: {state}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleAuth] Error al usar sesión: {ex.Message}");

            // Alternativa: usar una cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(15)
            };
            Response.Cookies.Append("GoogleAuthState", state, cookieOptions);
            Console.WriteLine($"[GoogleAuth] Estado guardado en cookie: {state}");
        }


        // Parámetros de OAuth
        // cargar client ID y client secret desde appsettings.json
        //var clientId = _configuration["Authentication:Google:ClientId"];
        var clientId = _configuration["Authentication:Google:ClientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            throw new InvalidOperationException("Google ClientId is not configured.");
        }
        var redirectUri = "https://localhost:5019/api/auth/google-callback";
        var scope = "email profile openid";

        var pkceValues = PKCE.Create(64, "S256");
        string[] parts = pkceValues.Split(',');
        string code_verifier = parts[0];
        string code_challenge = parts[1];

        HttpContext.Session.SetString("CodeVerifier", code_verifier);
        Console.WriteLine($"[GoogleAuth] Code Verifier guardado en sesión: {code_verifier}");
        // Construir la URL de autorización de Google
        var googleAuthUrl =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&response_type=code" +
            $"&scope={Uri.EscapeDataString(scope)}" +
            $"&code_challenge_method=S256" +
            $"&code_challenge={Uri.EscapeDataString(code_challenge)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            $"&include_granted_scopes=true";

         
        Console.WriteLine($"[GoogleAuth] Estado generado: {state}");
        Console.WriteLine($"[GoogleAuth] URL de redirección: {redirectUri}");

        return Redirect(googleAuthUrl);
    }


    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery(Name = "code")] string? code, [FromQuery] string state, [FromQuery] string? error, [FromQuery] string? cedula)
    {
        Console.WriteLine($"[GoogleCallback] Recibido - Estado: {state}");
        Console.WriteLine($"[GoogleCallback] Código: {(code != null ? code.Substring(0, Math.Min(10, code.Length)) + "..." : "null")}");
        Console.WriteLine($"[GoogleCallback] Error: {error}");
        Console.WriteLine($"[GoogleCallback] Cédula: {cedula}");

        // Variable para almacenar el estado guardado
        string? savedState = null;

        // Verificar si estamos en el flujo de verificación de cédula
        bool isReturningFromVerification = !string.IsNullOrEmpty(cedula);
        Console.WriteLine($"[GoogleCallback] ¿Retornando desde verificación de cédula? {isReturningFromVerification}");
        // Si viene de verificación pero no tiene código, no lo necesitamos
        if (isReturningFromVerification && string.IsNullOrEmpty(code))
        {
            Console.WriteLine("[GoogleCallback] Retornando de verificación sin código, esto es esperado");
            code = "not_required_for_verification"; // Valor dummy para evitar validaciones
        }
        else if (string.IsNullOrEmpty(code))
        {
            // Si no viene de verificación y no tiene código, es un error
            return BadRequest(new { message = "El parámetro 'code' es requerido para la autenticación inicial" });
        }

        try
        {
            // Intentar recuperar de la sesión
            savedState = HttpContext.Session.GetString("GoogleOAuthState");
            Console.WriteLine($"[GoogleCallback] Estado recuperado de sesión: {savedState}");
        }
        catch
        {
            // Si falla, intentar recuperar de la cookie
            if (Request.Cookies.TryGetValue("GoogleAuthState", out savedState))
            {
                Console.WriteLine($"[GoogleCallback] Estado recuperado de cookie: {savedState}");
            }
        }

        // Si no estamos retornando desde verificación, verificamos el estado normalmente
        if (!isReturningFromVerification)
        {
            // Verificar el estado solo si es la llamada inicial de OAuth
            if (string.IsNullOrEmpty(savedState) || state != savedState)
            {
                return Content($@"
            <html>
                <body>
                    <h1>Error de autenticación</h1>
                    <p>El estado no coincide o no está presente.</p>
                    <p>Estado recibido: {state}</p>
                    <p>Estado guardado: {savedState ?? "No encontrado"}</p>
                </body>
            </html>", "text/html");
            }

            // Flujo normal de autenticación con Google
            try
            {
                // Intercambiar el código por tokens
                var clientId = _configuration["Authentication:Google:ClientId"];
                var clientSecret = _configuration["Authentication:Google:ClientSecret"];
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new InvalidOperationException("Google ClientId or ClientSecret is not configured.");
                }
                var redirectUri = "https://localhost:5019/api/auth/google-callback" +
                    "";
                var code_verifier = HttpContext.Session.GetString("CodeVerifier");
                Console.WriteLine($"[GoogleCallback] Code Verifier recuperado de sesión: {code_verifier}");
                if (string.IsNullOrEmpty(code_verifier))
                {
                    Console.WriteLine("[GoogleCallback] Error: No se encontró el code_verifier en la sesión");
                    return Content("<h1>Error de autenticación</h1><p>No se pudo recuperar el verificador de código PKCE.</p>", "text/html");
                }

                using var httpClient = new HttpClient();
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret, // Ya no mandamos el client_secret porque usamos PKCE
                    ["redirect_uri"] = redirectUri,
                    ["code_verifier"] = code_verifier,
                    ["grant_type"] = "authorization_code"
                });

                var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
                var responseContent = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("CodeVerifier");
                    Console.WriteLine($"[GoogleCallback] Error en token: {responseContent}");
                    return Content($"<h1>Error al obtener token</h1><pre>{responseContent}</pre>", "text/html");
                }

                // Limpiar el code_verifier de la sesión
                HttpContext.Session.Remove("CodeVerifier");

                // Extraer el access_token y el id_token
                var responseJson = System.Text.Json.JsonDocument.Parse(responseContent);
                var accessToken = responseJson.RootElement.GetProperty("access_token").GetString();

                // Obtener información del usuario
                var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
                userInfoRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var userInfoResponse = await httpClient.SendAsync(userInfoRequest);
                var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    return Content($"<h1>Error al obtener datos del usuario</h1><pre>{userInfoContent}</pre>", "text/html");
                }

                // Procesar la información del usuario
                var userInfoJson = System.Text.Json.JsonDocument.Parse(userInfoContent);
                var claims = new List<Claim>();

                // Extraer claims comunes
                if (userInfoJson.RootElement.TryGetProperty("sub", out var subElement))
                    claims.Add(new Claim("sub", subElement.GetString() ?? string.Empty));

                if (userInfoJson.RootElement.TryGetProperty("email", out var emailElement))
                    claims.Add(new Claim("email", emailElement.GetString() ?? string.Empty));

                if (userInfoJson.RootElement.TryGetProperty("name", out var nameElement))
                    claims.Add(new Claim("name", nameElement.GetString() ?? string.Empty));

                // Guardar la información importante en la sesión para usarla cuando el usuario regrese
                HttpContext.Session.SetString("GoogleUserInfo", userInfoContent);
                HttpContext.Session.SetString("GoogleAccessToken", accessToken ?? string.Empty);

                Console.WriteLine("[GoogleCallback] No tiene cédula, redirigiendo al formulario de verificación");

                // No eliminamos el estado porque volveremos a este endpoint
                var tempToken = _jwtTokenService.GenerateJwtToken(claims);
                return Redirect($"http://localhost:3000/verify-age?token={Uri.EscapeDataString(tempToken)}&state={Uri.EscapeDataString(state)}&returnUrl=/auth-callback");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleCallback] Error: {ex.Message}");
                Console.WriteLine($"[GoogleCallback] Stack: {ex.StackTrace}");
                return Content($"<h1>Error en el proceso</h1><p>{ex.Message}</p><pre>{ex.StackTrace}</pre>", "text/html");
            }
        }
        else
        {
            // Flujo cuando el usuario regresa con la cédula
            Console.WriteLine($"[GoogleCallback] Retornando con cédula desde verificación");

            // Recuperar información guardada en la sesión
            var accessToken = HttpContext.Session.GetString("GoogleAccessToken");
            var userInfoContent = HttpContext.Session.GetString("GoogleUserInfo");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userInfoContent))
            {
                return Content($@"
            <html>
                <body>
                    <h1>Error de sesión</h1>
                    <p>No se pudo recuperar la información de autenticación.</p>
                    <p>Por favor, inicie el proceso de autenticación nuevamente.</p>
                </body>
            </html>", "text/html");
            }

            try
            {
                // Procesar la información del usuario
                var userInfoJson = System.Text.Json.JsonDocument.Parse(userInfoContent);
                var claims = new List<Claim>();

                // Extraer claims comunes
                if (userInfoJson.RootElement.TryGetProperty("sub", out var subElement))
                    claims.Add(new Claim("sub", subElement.GetString() ?? string.Empty));

                if (userInfoJson.RootElement.TryGetProperty("email", out var emailElement))
                    claims.Add(new Claim("email", emailElement.GetString() ?? string.Empty));

                if (userInfoJson.RootElement.TryGetProperty("name", out var nameElement))
                    claims.Add(new Claim("name", nameElement.GetString() ?? string.Empty));

                // Si tenemos cédula, verificamos la edad
                bool isAdult = false;
                try
                {
                    // Verificación de edad
                    var client = _httpClientFactory.CreateClient();
                    var verifyResponse = await client.GetAsync($"https://localhost:5019/api/verify/age_verify?cedula={Uri.EscapeDataString(cedula ?? string.Empty)}");

                    if (verifyResponse.IsSuccessStatusCode)
                    {
                        var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
                        var verifyResult = System.Text.Json.JsonSerializer.Deserialize<AgeVerificationResult>(
                            verifyContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (verifyResult != null)
                        {
                            isAdult = verifyResult.IsAllowed;
                            claims.Add(new Claim("edad", verifyResult.Edad.ToString()));
                            Console.WriteLine($"[GoogleCallback] Verificación de edad: {(isAdult ? "Mayor de edad" : "Menor de edad")}");
                        }
                        else
                        {
                            Console.WriteLine("[GoogleCallback] Error: El resultado de la verificación de edad es nulo.");
                            isAdult = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[GoogleCallback] Error al verificar edad: {verifyResponse.StatusCode}");
                        isAdult = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GoogleCallback] Error al verificar edad: {ex.Message}");
                    isAdult = false;
                }

                // Agregar claims adicionales
                claims.Add(new Claim("is_adult", isAdult.ToString().ToLower()));
                claims.Add(new Claim("cedula", cedula ?? string.Empty));
                claims.Add(new Claim("google_access_token", accessToken));
                claims.Add(new Claim("auth_provider", "google"));
                claims.Add(new Claim("role", "user"));

                // Limpiamos datos de sesión que ya no necesitamos
                HttpContext.Session.Remove("GoogleOAuthState");
                HttpContext.Session.Remove("GoogleUserInfo");
                HttpContext.Session.Remove("GoogleAccessToken");

                Console.WriteLine("[GoogleCallback] Proceso completado, se elimina el estado de la sesión");

                // Generar token JWT
                var token = _jwtTokenService.GenerateJwtToken(claims);

                // Redireccionar al frontend con el token
                Console.WriteLine($"[GoogleCallback] Token JWT generado exitosamente");
                return Redirect($"http://localhost:3000/auth-callback?token={Uri.EscapeDataString(token)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleCallback] Error: {ex.Message}");
                Console.WriteLine($"[GoogleCallback] Stack: {ex.StackTrace}");
                return Content($"<h1>Error en el proceso</h1><p>{ex.Message}</p><pre>{ex.StackTrace}</pre>", "text/html");
            }
        }
    }



    // Clase para deserializar el resultado de la verificación de edad
    private class AgeVerificationResult
    {
        public bool IsAllowed { get; set; }
        public int Edad { get; set; }
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

    [HttpGet("userinfo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Requiere token JWT válido
    public IActionResult GetUserInfo()
    {
        // Extrae los claims del usuario desde el token JWT proporcionado en el header
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(userClaims);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var claims = new List<Claim>(); 
        try
        {
            //Requerimos como obligatorios el username y password
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña son requeridos" });
            }

            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Verificar si el usuario existe
            if (usuario == null)
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos" });
            }

            // Verificar la contraseña
            bool passwordValid = VerifyPassword(request.Password, usuario.Password);
            if (!passwordValid)
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos" });
            }

            // Si tenemos cédula, verificamos la edad
            bool isAdult = false;
            try
            {
                //me traigo el usuario de la base de datos para poder cargar los claims
                var usuarioFromDb = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (usuarioFromDb == null)
                {
                    Console.WriteLine("[SignIn] Error: usuarioFromDb es null.");
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Verificación de edad
                var client = _httpClientFactory.CreateClient();
                var ciString = usuarioFromDb.Ci.ToString();
                var verifyResponse = await client.GetAsync($"https://localhost:5019/api/verify/age_verify?cedula={Uri.EscapeDataString(ciString)}");

                if (verifyResponse.IsSuccessStatusCode)
                {
                    var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
                    var verifyResult = System.Text.Json.JsonSerializer.Deserialize<AgeVerificationResult>(
                        verifyContent, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (verifyResult != null)
                    {
                        isAdult = verifyResult.IsAllowed;
                        claims.Add(new Claim("edad", verifyResult.Edad.ToString()));
                        Console.WriteLine($"[SignIn] Verificación de edad: {(isAdult ? "Mayor de edad" : "Menor de edad")}");
                    }
                    else
                    {
                        Console.WriteLine("[SignIn] Error: El resultado de la verificación de edad es nulo.");
                        isAdult = false;
                    }
                }
                else
                {
                    Console.WriteLine($"[SignIn] Error al verificar edad: {verifyResponse.StatusCode}");
                    isAdult = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignIn] Error al verificar edad: {ex.Message}");
                isAdult = false;
            }
            // Autenticación exitosa, creamos claims para el JWT
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.Name, usuario.Nombre),
            //     new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
            //     new Claim("UserId", usuario.Id.ToString()),
            //     new Claim("role", usuario.Rol ?? "user")  // Usar el rol del usuario o "user" por defecto
            // };

            claims.Add(new Claim(ClaimTypes.Name, usuario.Nombre ?? string.Empty));
            claims.Add(new Claim("is_adult", isAdult.ToString().ToLower()));
            claims.Add(new Claim("role", usuario.Rol.ToString())); // Usar el rol del usuario o "user" por defecto


            // Si hay atributos adicionales que quieras incluir en el token
            if (usuario != null)
            {
                claims.Add(new Claim("cedula", usuario.Ci.ToString() ?? string.Empty));
            }

            // Generar el token JWT
            var token = _jwtTokenService.GenerateJwtToken(claims);

            // Devolver token y datos básicos del usuario
            return Ok(new
            {
                token,
                userId = usuario!.Id,
                username = usuario.Nombre,
                email = usuario.Email,
                role = usuario.Rol,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignIn] Error: {ex.Message}");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
    
    // Método auxiliar para verificar la contraseña
private bool VerifyPassword(string providedPassword, string storedPassword)
{
    // Suponiendo que las contraseñas están hasheadas con BCrypt
    return BCrypt.Net.BCrypt.Verify(providedPassword, storedPassword);
}

    // Clase para recibir los datos de inicio de sesión
    public class SignInRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // 1. Primero intentamos obtener el token desde el header de autorización
            string? jwtToken = null;
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var authHeaderValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeaderValue) && authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    jwtToken = authHeaderValue.Substring("Bearer ".Length).Trim();
                    Console.WriteLine($"[Logout] Token JWT recibido: {jwtToken.Substring(0, Math.Min(20, jwtToken.Length))}...");
                }
            }

            // 2. Si tenemos un token JWT, intentamos obtener información del usuario
            if (!string.IsNullOrEmpty(jwtToken))
            {
                try
                {
                    // Decodificar el token para obtener información
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(jwtToken) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

                    if (jsonToken != null)
                    {
                        // Buscar si hay claims que indiquen que fue un login con Google
                        var subClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub");
                        var emailClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "email");
                        var googleAccessTokenClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "google_access_token");

                        if (googleAccessTokenClaim != null)
                        {
                            Console.WriteLine("[Logout] Detectado token de acceso de Google, revocando sesión...");

                            // Llamar al endpoint de revocación de Google con el token de acceso
                            using (var httpClient = new HttpClient())
                            {
                                var revocationUrl = $"https://oauth2.googleapis.com/revoke?token={googleAccessTokenClaim.Value}";
                                Console.WriteLine($"[Logout] Llamando a URL de revocación: {revocationUrl}");

                                var revocationResponse = await httpClient.PostAsync(
                                    "https://oauth2.googleapis.com/revoke",
                                    new FormUrlEncodedContent(new Dictionary<string, string>
                                    {
                                        ["token"] = googleAccessTokenClaim.Value
                                    })
                                );

                                var responseContent = await revocationResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"[Logout] Respuesta de revocación: {revocationResponse.StatusCode}, Contenido: {responseContent}");

                                if (revocationResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("[Logout] Token de Google revocado exitosamente");
                                }
                                else
                                {
                                    Console.WriteLine($"[Logout] Error al revocar token de Google: {responseContent}");
                                }
                            }
                        }
                        else if (subClaim != null || (emailClaim != null && emailClaim.Value.EndsWith("@gmail.com")))
                        {
                            Console.WriteLine("[Logout] Detectado login con Google, pero no se encontró token de acceso en el JWT");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Logout] Error al procesar token JWT: {ex.Message}");
                    Console.WriteLine($"[Logout] Stack trace: {ex.StackTrace}");
                }
            }

            // 3. Como backup, también intentamos el método original con cookies
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult.Succeeded)
            {
                var accessToken = authenticateResult.Properties?.GetTokenValue("access_token");
                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Llamamos al endpoint de revocación de Google para invalidar el token
                    using (var httpClient = new HttpClient())
                    {
                        await httpClient.GetAsync($"https://oauth2.googleapis.com/revoke?token={accessToken}");
                        Console.WriteLine("[Logout] Token de Google revocado exitosamente");
                    }
                }
            }

            // 4. Limpiamos las cookies de autenticación en cualquier caso
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { message = "Sesión cerrada exitosamente" });
        }
        catch (Exception ex)
        {
            // Registramos el error pero devolvemos una respuesta de éxito de todos modos
            Console.WriteLine($"[Logout] Error al revocar el token: {ex.Message}");
            return Ok(new { message = "Sesión cerrada con advertencias", warning = ex.Message });
        }
    }

}
