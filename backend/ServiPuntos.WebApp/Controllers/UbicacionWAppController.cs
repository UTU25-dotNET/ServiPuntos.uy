using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "AdminTenant")]
        public async Task<IActionResult> Administrar()
        {
            try
            {
                // Obtener el tenantId del usuario logueado
                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                
                if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
                {
                    TempData["Error"] = "No se pudo identificar su tenant. Por favor, inicie sesión nuevamente.";
                    return RedirectToAction("Index", "Home");
                }

                // Obtener información del tenant
                var tenant = await _tenantService.GetByIdAsync(tenantId);
                if (tenant == null)
                {
                    TempData["Error"] = "Tenant no encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                // Obtener ubicaciones del tenant
                var ubicaciones = await _ubicacionRepository.GetAllAsync(tenantId);

                ViewBag.TenantName = tenant.Nombre;
                ViewBag.TenantId = tenantId;

                return View(ubicaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar ubicaciones: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
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
            // Si es AdminTenant, no necesita lista de tenants
            if (!User.IsInRole("AdminTenant"))
            {
                ViewBag.Tenants = await _tenantService.GetAllAsync();
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Ubicacion ubicacion)
        {
            // Si es AdminTenant, asignar automáticamente su tenant
            if (User.IsInRole("AdminTenant"))
            {
                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
                {
                    TempData["Error"] = "No se pudo identificar su tenant.";
                    return RedirectToAction("Administrar");
                }
                ubicacion.TenantId = tenantId;
            }

            if (!ModelState.IsValid)
            {
                // Recargar tenants si no es AdminTenant
                if (!User.IsInRole("AdminTenant"))
                {
                    ViewBag.Tenants = await _tenantService.GetAllAsync();
                }
                return View(ubicacion);
            }

             ubicacion.FechaCreacion = DateTime.UtcNow;
             ubicacion.FechaModificacion = DateTime.UtcNow;

            try
            {
                await _ubicacionRepository.AddAsync(ubicacion.TenantId, ubicacion);
                
                // Si es AdminTenant, redirigir a Administrar, sino a Index
                if (User.IsInRole("AdminTenant"))
                {
                    TempData["Success"] = "Ubicación creada exitosamente.";
                    return RedirectToAction("Administrar");
                }
                
                TempData["Success"] = "Ubicación creada exitosamente.";
                return RedirectToAction("Index", new { tenantId = ubicacion.TenantId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear la ubicación: {ex.Message}";
                
                // Recargar tenants si no es AdminTenant
                if (!User.IsInRole("AdminTenant"))
                {
                    ViewBag.Tenants = await _tenantService.GetAllAsync();
                }
                return View(ubicacion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            // Si es AdminTenant, verificar que la ubicación pertenezca a su tenant
            if (User.IsInRole("AdminTenant"))
            {
                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
                {
                    TempData["Error"] = "No se pudo verificar su tenant.";
                    return RedirectToAction("Administrar");
                }

                if (ubicacion.TenantId != tenantId)
                {
                    TempData["Error"] = "No tiene permisos para editar esta ubicación.";
                    return RedirectToAction("Administrar");
                }
            }

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

            // Si es AdminTenant, redirigir a Administrar, sino a Index
            if (User.IsInRole("AdminTenant"))
            {
                TempData["Success"] = "Ubicación actualizada exitosamente.";
                return RedirectToAction("Administrar");
            }

            return RedirectToAction("Index", new { tenantId = ubicacion.TenantId });
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            // Si es AdminTenant, verificar que la ubicación pertenezca a su tenant
            if (User.IsInRole("AdminTenant"))
            {
                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out Guid tenantId))
                {
                    TempData["Error"] = "No se pudo verificar su tenant.";
                    return RedirectToAction("Administrar");
                }

                if (ubicacion.TenantId != tenantId)
                {
                    TempData["Error"] = "No tiene permisos para eliminar esta ubicación.";
                    return RedirectToAction("Administrar");
                }
            }

            return View(ubicacion);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> ConfirmarEliminar(Guid id)
        {
            var ubicacion = await _ubicacionRepository.GetAsync(id);
            if (ubicacion == null)
                return NotFound();

            var tenantId = ubicacion.TenantId;
            await _ubicacionRepository.DeleteAsync(ubicacion.TenantId, ubicacion.Id);

            // Si es AdminTenant, redirigir a Administrar, sino a Index
            if (User.IsInRole("AdminTenant"))
            {
                TempData["Success"] = "Ubicación eliminada exitosamente.";
                return RedirectToAction("Administrar");
            }

            return RedirectToAction("Index", new { tenantId = tenantId });
<<<<<<< HEAD
=======
        }

        [HttpGet]
        [Authorize(Roles = "AdminUbicacion")]
        public async Task<IActionResult> GestionarUbicacion()
        {
            var ubicacionIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ubicacionId")?.Value;
            if (string.IsNullOrEmpty(ubicacionIdClaim) || !Guid.TryParse(ubicacionIdClaim, out Guid ubicacionId))
            {
                return RedirectToAction("Index", "DashboardWApp");
            }

            var ubicacion = await _ubicacionRepository.GetAsync(ubicacionId);
            if (ubicacion == null)
            {
                return RedirectToAction("Index", "DashboardWApp");
            }

            return View(ubicacion);
        }

        [HttpPost]
        [Authorize(Roles = "AdminUbicacion")]
        public async Task<IActionResult> GestionarUbicacion(Ubicacion model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

                        var ubicacion = await _ubicacionRepository.GetAsync(model.Id);
            if (ubicacion == null)
            {
                return RedirectToAction("Index", "DashboardWApp");
            }

            // Actualizar solo los campos configurables desde la vista
            ubicacion.HoraApertura = model.HoraApertura;
            ubicacion.HoraCierre = model.HoraCierre;
            ubicacion.Lavado = model.Lavado;
            ubicacion.CambioDeAceite = model.CambioDeAceite;
            ubicacion.CambioDeNeumaticos = model.CambioDeNeumaticos;
            ubicacion.PrecioLavado = model.PrecioLavado;
            ubicacion.PrecioCambioAceite = model.PrecioCambioAceite;
            ubicacion.PrecioCambioNeumaticos = model.PrecioCambioNeumaticos;
            ubicacion.FechaModificacion = DateTime.UtcNow;

            await _ubicacionRepository.UpdateAsync(model.TenantId, ubicacion);
            TempData["Success"] = "Ubicación actualizada.";
            return RedirectToAction("Index", "DashboardWApp");
>>>>>>> origin/dev
        }
    }
}