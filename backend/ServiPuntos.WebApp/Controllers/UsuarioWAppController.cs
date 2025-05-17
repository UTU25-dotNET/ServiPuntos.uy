using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
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

        [HttpGet]
        public async Task<IActionResult> Index(Guid? tenantId)
        {
            var tenants = await _iTenantService.GetAllAsync();
            ViewBag.Tenants = tenants;

            if (!tenantId.HasValue)
            {
                // No hay tenant seleccionado aún, no mostramos usuarios, solo el dropdown
                return View(new List<Usuario>());
            }

            // Tenant seleccionado, traemos usuarios
            var usuarios = await _iUsuarioService.GetAllUsuariosAsync(tenantId.Value);
        
            ViewBag.TenantSeleccionado = tenantId;

            return View(usuarios);
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
        public async Task<IActionResult> Crear(string nombre, string email, string password, Guid tenantId, RolUsuario rol)
        {
            if (ModelState.IsValid) {          
                
                var usuario = new Usuario(nombre, email, password, tenantId, rol);

                await _iUsuarioService.AddUsuarioAsync(usuario);
                //return RedirectToAction("Index");
            }
            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            return View();
        }
    }

    internal class UsuarioIndexViewModel
    {
        public List<Tenant> Tenants { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public Guid? TenantSeleccionado { get; set; }
    }
}
