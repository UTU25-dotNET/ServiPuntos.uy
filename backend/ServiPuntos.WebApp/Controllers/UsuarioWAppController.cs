using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers
{
    public class UsuarioWAppController : Controller
    {

        private readonly IUsuarioService _iUsuarioService;
        private readonly ITenantContext _iTenantContext;
        private readonly ITenantService _iTenantService;

        public UsuarioWAppController(IUsuarioService usuarioService, ITenantContext tenantContext, ITenantService iTenantService)
        {
            _iUsuarioService = usuarioService;
            _iTenantContext = tenantContext;
            _iTenantService = iTenantService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            // Obtiene todas las tenants desde tu servicio
            var tenants = await _iTenantService.GetAllAsync();
            // Pásalas a la vista, por ejemplo en ViewBag
            ViewBag.Tenants = tenants;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string email, string password, Guid tenantId)
        {
            if (ModelState.IsValid) {             
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
                    TenantId = tenantId
                };
                await _iUsuarioService.AddUsuarioAsync(usuario);
                return RedirectToAction("Index");
            }
            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            return View();
        }
    }
}
