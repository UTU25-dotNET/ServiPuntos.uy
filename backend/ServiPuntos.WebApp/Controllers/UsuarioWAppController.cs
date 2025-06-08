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

            // Si no hay tenant seleccionado, usar el primero por defecto
            if (!tenantId.HasValue && tenants.Any())
            {
                tenantId = tenants.First().Id;
            }

            ViewBag.TenantSeleccionado = tenantId;

            // Obtener usuarios del tenant seleccionado
            IEnumerable<Usuario> usuarios = new List<Usuario>();
            if (tenantId.HasValue)
            {
                usuarios = await _iUsuarioService.GetAllUsuariosAsync(tenantId.Value);
            }

            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener información del tenant para mostrar el nombre
            var tenant = await _iTenantService.GetByIdAsync(usuario.TenantId);
            ViewBag.TenantNombre = tenant?.Nombre ?? "Tenant no encontrado";

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            
            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Guid id, string nombre, string email, int ci, Guid tenantId, RolUsuario rol)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _iUsuarioService.GetUsuarioAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Actualizar propiedades del usuario
                usuario.Nombre = nombre;
                usuario.Email = email;
                usuario.Ci = ci;
                usuario.TenantId = tenantId;
                usuario.Rol = rol;

                await _iUsuarioService.UpdateUsuarioAsync(usuario);
                return RedirectToAction("Index", new { tenantId = tenantId });
            }

            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            var usuarioModel = await _iUsuarioService.GetUsuarioAsync(id);
            return View(usuarioModel);
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener información del tenant para mostrar el nombre
            var tenant = await _iTenantService.GetByIdAsync(usuario.TenantId);
            ViewBag.TenantNombre = tenant?.Nombre ?? "Tenant no encontrado";

            return View(usuario);
        }

        [HttpPost, ActionName("Borrar")]
        public async Task<IActionResult> BorrarConfirmed(Guid id)
        {
            var usuario = await _iUsuarioService.GetUsuarioAsync(id);
            if (usuario != null)
            {
                var tenantId = usuario.TenantId;
                await _iUsuarioService.DeleteUsuarioAsync(id);
                return RedirectToAction("Index", new { tenantId = tenantId });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string email, string password, int ci, Guid tenantId, RolUsuario rol)
        {
            if (ModelState.IsValid) {          
                
                var usuario = new Usuario(nombre, email, password, ci, tenantId, rol);

                await _iUsuarioService.AddUsuarioAsync(usuario);
                // Redirigir de vuelta al Index con el tenant seleccionado
                return RedirectToAction("Index", new { tenantId = tenantId });
            }
            ViewBag.Tenants = await _iTenantService.GetAllAsync();
            return View();
        }
    }
}