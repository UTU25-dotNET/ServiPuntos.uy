using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers
{
    public class AccountWAppController : Controller
    {
        private readonly IUsuarioService _iUsuarioService;

        public AccountWAppController(IUsuarioService usuarioService)
        {
            _iUsuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = "/")
        {

            var usuario = await _iUsuarioService.ValidarCredencialesAsync(email, password);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("tenantId", usuario.TenantId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Configurar la autenticación de cookies
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantener la sesión activa
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Tiempo de expiración
            };
  
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (Url.IsLocalUrl(returnUrl))
            {
                Console.WriteLine("...");
                return Redirect(returnUrl);
            }
            else
            {
                Console.WriteLine("✅ Login exitoso, redirigiendo...");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "AccountWApp");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
