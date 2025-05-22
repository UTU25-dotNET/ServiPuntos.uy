using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers
{
    public class UbicacionWAppController : Controller
    {
        private readonly IUbicacionRepository _ubicacionRepository;
        private readonly ITenantService _tenantService;

        public UbicacionWAppController(IUbicacionRepository ubicacionRepository, ITenantService tenantService)
        {
            _ubicacionRepository = ubicacionRepository;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? tenantId)
        {
            var tenants = await _tenantService.GetAllAsync();
            ViewBag.Tenants = tenants;

            if (!tenantId.HasValue)
                return View(new List<Ubicacion>());

            var ubicaciones = await _ubicacionRepository.GetAllAsync(tenantId.Value);
            ViewBag.TenantSeleccionado = tenantId;

            return View(ubicaciones);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            return View(ubicacion);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Tenants = await _tenantService.GetAllAsync();
            //return View(new Ubicacion());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Ubicacion ubicacion)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tenants = await _tenantService.GetAllAsync();
                return View(ubicacion);
            }

            ubicacion.FechaCreacion = DateTime.UtcNow;
            ubicacion.FechaModificacion = DateTime.UtcNow;

            await _ubicacionRepository.AddAsync(ubicacion.TenantId, ubicacion);
            return RedirectToAction("Index", new { tenantId = ubicacion.TenantId });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            ViewBag.Tenants = await _tenantService.GetAllAsync();
            return View(ubicacion);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Ubicacion ubicacion)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tenants = await _tenantService.GetAllAsync();
                return View(ubicacion);
            }

            ubicacion.FechaModificacion = DateTime.UtcNow;
            await _ubicacionRepository.UpdateAsync(ubicacion.TenantId, ubicacion);

            return RedirectToAction("Index", new { tenantId = ubicacion.TenantId });
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            return View(ubicacion);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> ConfirmarEliminar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            await _ubicacionRepository.DeleteAsync(ubicacion.TenantId, ubicacion.Id);
            return RedirectToAction("Index", new { tenantId = ubicacion.TenantId });
        }
    }
}
