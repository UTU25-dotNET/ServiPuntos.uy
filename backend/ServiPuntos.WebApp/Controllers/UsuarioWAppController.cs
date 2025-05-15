using Microsoft.AspNetCore.Mvc;

namespace ServiPuntos.WebApp.Controllers
{
    public class UsuarioWAppController : Controller
    {

        private readonly IUsuarioService _iUsuarioService;
        private readonly ITenantContext _iTenantContext;

        public UsuarioWAppController(IUsuarioService usuarioService, ITenantContext tenantContext)
        {
            _iUsuarioService = usuarioService;
            _iTenantContext = tenantContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string email, string password)
        {
            if (ModelState.IsValid) {             
                Guid tenant = _iTenantContext.TenantId;

                //var usuario = new Usuario(nombre, email, password, tenant);
                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = nombre,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Puntos = 0,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    TenantId = tenant
                };
                await _iUsuarioService.AddUsuarioAsync(usuario);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
