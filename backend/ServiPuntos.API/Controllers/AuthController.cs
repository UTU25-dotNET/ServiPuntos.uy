using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

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
    private readonly ServiPuntosDbContext _context;
    
    private readonly ITenantService _tenantService;

    /// <summary>
    /// Constructor que inyecta las dependencias necesarias
    /// </summary>
    /// <param name="jwtTokenService">Servicio para generar tokens JWT</param>
    public AuthController(JwtTokenService jwtTokenService, IConfiguration configuration, IHttpClientFactory httpClientFactory, ServiPuntosDbContext context, ITenantService tenantService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _tenantService = tenantService;

    }

    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants()
    {
        try
        {
            Console.WriteLine("[GetTenants] Obteniendo lista de tenants...");
            
            // **USAR ITenantService en lugar del contexto directo**
            var allTenants = await _tenantService.GetAllAsync();
            
            // Filtrar solo los activos y mapear a la respuesta
            var tenants = allTenants // Solo tenants activos
                .Select(t => new
                {
                    id = t.Id,
                    nombre = t.Nombre,
                })
                .OrderBy(t => t.nombre)
                .ToList();

            Console.WriteLine($"[GetTenants] Se encontraron {tenants.Count} tenants activos");
            return Ok(tenants);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetTenants] Error: {ex.Message}");
            return StatusCode(500, new { message = "Error al obtener la lista de tenants" });
        }
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

        string? savedState;

        if (string.IsNullOrEmpty(code))
        {
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

        // Verificar el estado
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

        try
        {
            // Intercambiar el código por tokens
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("Google ClientId or ClientSecret is not configured.");
            }
            var redirectUri = "https://localhost:5019/api/auth/google-callback";
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
                ["client_secret"] = clientSecret,
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

            // Extraer el access_token
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

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == userInfoJson.RootElement.GetProperty("email").GetString());

            if (usuario == null)
            {
                // Redirigir al login con un parametro en la URL que diga que el usuario no existe y debe registrarse
                Console.WriteLine("[GoogleCallback] Usuario no encontrado en la base de datos, redirigiendo al registro...");
                return Redirect($"http://localhost:3000/login?error=Usuario+no+registrado&email={Uri.EscapeDataString(userInfoJson.RootElement.GetProperty("email").GetString() ?? string.Empty)}");
            }
            else
            {
                // Si el usuario ya existe, actualizamos su información
                Console.WriteLine("[GoogleCallback] Usuario encontrado, actualizando información...");
                usuario.Nombre = userInfoJson.RootElement.GetProperty("name").GetString() ?? string.Empty;
                usuario.Email = userInfoJson.RootElement.GetProperty("email").GetString() ?? string.Empty;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                claims.Add(new Claim("TenantId", usuario.TenantId.ToString() ?? string.Empty));
            }

            // Guardar la información importante en la sesión para usarla cuando el usuario regrese
            HttpContext.Session.SetString("GoogleUserInfo", userInfoContent);
            HttpContext.Session.SetString("GoogleAccessToken", accessToken ?? string.Empty);

            claims.Add(new Claim("google_access_token", accessToken ?? string.Empty));

            //Cargamos cookie con claims
            Console.WriteLine("[GoogleCallback] Generando cookies de autenticación...");
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            Console.WriteLine("[GoogleCallback] Usuario autenticado con cookies");

            // No eliminamos el estado porque volveremos a este endpoint
            var tempToken = await _jwtTokenService.GenerateJwtTokenAsync(claims);
            return Redirect($"http://localhost:3000/auth-callback?token={Uri.EscapeDataString(tempToken)}&state={Uri.EscapeDataString(state)}&returnUrl=/auth-callback");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleCallback] Error: {ex.Message}");
            Console.WriteLine($"[GoogleCallback] Stack: {ex.StackTrace}");
            return Content($"<h1>Error en el proceso</h1><p>{ex.Message}</p><pre>{ex.StackTrace}</pre>", "text/html");
        }
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado mediante cookies de sesión
    /// </summary>
    [HttpGet("session-userinfo")]
    public async Task<IActionResult> GetSessionUserInfo()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return Unauthorized();

        var userClaims = authenticateResult.Principal.Claims.Select(c => new { c.Type, c.Value });
        return Ok(userClaims);
    }

    [HttpGet("userinfo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetUserInfo()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(userClaims);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var claims = new List<Claim>(); 
        try
        {
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

            claims.Add(new Claim(ClaimTypes.Name, usuario.Nombre ?? string.Empty));
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty));
            claims.Add(new Claim("is_adult", isAdult.ToString().ToLower()));
            claims.Add(new Claim("role", usuario.Rol.ToString()));

            // // Si hay atributos adicionales que quieras incluir en el token
            // if (usuario != null && !string.IsNullOrEmpty(usuario.Ci))
            // {
            //     claims.Add(new Claim("cedula", usuario.Ci));
            // }

            // Generar el token JWT
            var token = await _jwtTokenService.GenerateJwtTokenAsync(claims);

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
    

    [HttpPost("verify-password")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email y contraseña son requeridos" });
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        bool passwordValid = VerifyPassword(request.Password, usuario.Password);
        if (!passwordValid)
        {
            return Unauthorized(new { message = "Contraseña incorrecta" });
        }

        return Ok(new { message = "Contraseña válida" });
    }

    // Método auxiliar para verificar la contraseña
    private bool VerifyPassword(string providedPassword, string storedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, storedPassword);
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del usuario a registrar</param>
    /// <returns>Resultado del registro</returns>
   [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        Console.WriteLine("[Register] Iniciando proceso de registro...");
        Console.WriteLine($"Register request received: {request?.Email ?? "null"}");
        try
        {
            Console.WriteLine($"[Register] Iniciando registro para: {request?.Email}");

            // Validar datos de entrada
            if (request == null)
            {
                return BadRequest(new { message = "Los datos de registro son requeridos" });
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new { message = "El nombre es requerido" });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "El email es requerido" });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "La contraseña es requerida" });
            }

            if (string.IsNullOrWhiteSpace(request.Ci))
            {
                return BadRequest(new { message = "La Cédula de Identidad es requerida" });
            }

            // **NUEVA VALIDACIÓN: TenantId obligatorio**
            if (request.TenantId == Guid.Empty)
            {
                return BadRequest(new { message = "Debe seleccionar un tenant" });
            }

            // Validar formato de email
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "El formato del email no es válido" });
            }

            // Validar fortaleza de la contraseña
            if (request.Password.Length < 6)
            {
                return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });
            }

            // Validar formato de CI
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Ci, @"^\d{7,8}$"))
            {
                return BadRequest(new { message = "La Cédula de Identidad debe tener entre 7 y 8 dígitos" });
            }

            // **USAR ITenantService para verificar que el tenant existe y está activo**
            var tenant = await _tenantService.GetByIdAsync(request.TenantId);

            if (tenant == null)
            {
                Console.WriteLine($"[Register] Tenant no válido o inactivo: {request.TenantId}");
                return BadRequest(new { message = "El tenant seleccionado no es válido" });
            }

            // Verificar si el usuario ya existe por email
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                Console.WriteLine($"[Register] Usuario ya existe con email: {request.Email}");
                return BadRequest(new { message = "Ya existe un usuario registrado con este email" });
            }

            // Verificar si la CI ya existe
            var existingUserByCi = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Ci.ToString() == request.Ci);

            if (existingUserByCi != null)
            {
                Console.WriteLine($"[Register] Usuario ya existe con CI: {request.Ci}");
                return BadRequest(new { message = "Ya existe un usuario registrado con esta Cédula de Identidad" });
            }

            // Hash de la contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Crear nuevo usuario con tenant seleccionado
            var newUser = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Email = request.Email.ToLower().Trim(),
                Password = hashedPassword,
                Ci = int.Parse(request.Ci.Trim()),
                TenantId = request.TenantId, // **USAR Guid del tenant seleccionado**
                Rol = RolUsuario.UsuarioFinal,
                FechaCreacion = DateTime.UtcNow,
            };

            // Guardar en la base de datos
            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[Register] Usuario creado exitosamente con ID: {newUser.Id} en Tenant: {tenant.Nombre}");

            // Respuesta exitosa
            return Ok(new
            {
                message = "Usuario registrado exitosamente",
                userId = newUser.Id,
                email = newUser.Email,
                nombre = newUser.Nombre,
                ci = newUser.Ci,
                tenantId = newUser.TenantId,
                tenantNombre = tenant.Nombre,
                fechaCreacion = newUser.FechaCreacion
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Register] Error en registro: {ex.Message}");
            Console.WriteLine($"[Register] Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Error interno del servidor al registrar usuario" });
        }
    }

    // Actualizar RegisterRequest para usar Guid
    public class RegisterRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        Console.WriteLine("[Logout] Iniciando proceso de cierre de sesión...");
        try
        {
            // 1. Primero intentamos obtener el token desde el header de autorización
            string? jwtToken = null;
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                Console.WriteLine("[Logout] Intentando obtener token desde el header de autorización...");
                var authHeaderValue = authHeader.FirstOrDefault();
                Console.WriteLine($"[Logout] Header de autorización: {authHeaderValue}");
                if (!string.IsNullOrEmpty(authHeaderValue) && authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    jwtToken = authHeaderValue.Substring("Bearer ".Length).Trim();
                    Console.WriteLine($"[Logout] Token JWT recibido: {jwtToken.Substring(0, Math.Min(20, jwtToken.Length))}...");
                }
                Console.WriteLine("[Logout] Token obtenido del header de autorización");
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
                        var googleAccessTokenClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "google_access_token");

                        if (googleAccessTokenClaim != null)
                        {
                            Console.WriteLine("[Logout] Detectado token de acceso de Google, revocando sesión...");

                            // Llamar al endpoint de revocación de Google con el token de acceso
                            using (var httpClient = new HttpClient())
                            {
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

    /// <summary>
    /// Clase para recibir los datos de inicio de sesión
    /// </summary>
    public class SignInRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    /// <summary>
    /// Clase para recibir los datos de verificación de contraseña
    /// </summary>
    public class VerifyPasswordRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    /// <summary>
    /// Método auxiliar para validar formato de email
    /// </summary>
    /// <param name="email">Email a validar</param>
    /// <returns>True si el formato es válido</returns>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}