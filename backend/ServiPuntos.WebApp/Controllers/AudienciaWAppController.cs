using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.DTOs;

namespace ServiPuntos.WebApp.Controllers
{
    // [Authorize(Roles = "AdminTenant")]
    public class AudienciaWAppController : Controller
    {
        private readonly IAudienciaService _iAudienciaService;
        private readonly ITenantService _iTenantService;
        private readonly IUsuarioService _iUsuarioService;
        private readonly ITenantContext _iTenantContext;

        public AudienciaWAppController(
            IAudienciaService audienciaService,
            ITenantService tenantService,
            IUsuarioService usuarioService,
            ITenantContext tenantContext = null)
        {
            _iAudienciaService = audienciaService;
            _iTenantService = tenantService;
            _iUsuarioService = usuarioService;
            _iTenantContext = tenantContext;
        }

        /// <summary>
        /// Obtiene el TenantId efectivo, priorizando el claim del usuario autenticado
        /// </summary>
        private Guid GetEffectiveTenantId(Guid? tenantIdFromRoute = null)
        {
            // Si es AdminTenant, usar su propio tenant
            if (User.IsInRole("AdminTenant"))
            {
                var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out Guid userTenantId))
                {
                    return userTenantId;
                }
            }

            // Si viene de la ruta y es AdminPlataforma
            if (tenantIdFromRoute.HasValue && tenantIdFromRoute.Value != Guid.Empty)
            {
                return tenantIdFromRoute.Value;
            }

            // Fallback al contexto
            if (_iTenantContext?.TenantId != Guid.Empty)
            {
                return _iTenantContext.TenantId;
            }

            throw new InvalidOperationException("No se pudo determinar el TenantId.");
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? tenantId)
        {
            try
            {
                var tenants = await _iTenantService.GetAllAsync();
                ViewBag.Tenants = tenants;

                // Obtener tenant efectivo
                Guid effectiveTenantId;
                try
                {
                    effectiveTenantId = GetEffectiveTenantId(tenantId);
                }
                catch (InvalidOperationException)
                {
                    // Si no se puede determinar el tenant y hay tenants disponibles, usar el primero (solo AdminPlataforma)
                    if (tenants.Any() && User.IsInRole("AdminPlataforma"))
                    {
                        effectiveTenantId = tenants.First().Id;
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo identificar el tenant.";
                        return View(new List<Audiencia>());
                    }
                }

                ViewBag.TenantSeleccionado = effectiveTenantId;

                // Obtener audiencias del tenant seleccionado
                var audiencias = await _iAudienciaService.GetDefinicionesAudienciaAsync(effectiveTenantId);

                return View(audiencias);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar audiencias: {ex.Message}";
                Console.WriteLine($"❌ Error en Index: {ex.Message}");
                return View(new List<Audiencia>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(Guid id)
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();
                
                // Obtener la audiencia específica
                var audiencias = await _iAudienciaService.GetDefinicionesAudienciaAsync(tenantId);
                var audiencia = audiencias.FirstOrDefault(a => a.Id == id);

                if (audiencia == null)
                {
                    TempData["Error"] = "Audiencia no encontrada.";
                    return RedirectToAction("Index", new { tenantId = tenantId });
                }

                // Obtener información del tenant
                var tenant = await _iTenantService.GetByIdAsync(tenantId);
                ViewBag.TenantNombre = tenant?.Nombre ?? "Tenant no encontrado";

                // Obtener usuarios de la audiencia
                var usuarios = await _iAudienciaService.GetUsuariosPorAudienciaAsync(tenantId, audiencia.NombreUnicoInterno);
                ViewBag.UsuariosAudiencia = usuarios;
                ViewBag.TotalUsuarios = usuarios.Count();

                // Obtener estadísticas
                var estadisticas = await _iAudienciaService.GetEstadisticasGlobalesYAporAudienciaAsync(tenantId);
                ViewBag.Estadisticas = estadisticas;

                return View(audiencia);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar detalles de la audiencia: {ex.Message}";
                Console.WriteLine($"❌ Error en Detalles: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();
                
                // Obtener la audiencia específica
                var audiencias = await _iAudienciaService.GetDefinicionesAudienciaAsync(tenantId);
                var audiencia = audiencias.FirstOrDefault(a => a.Id == id);

                if (audiencia == null)
                {
                    TempData["Error"] = "Audiencia no encontrada.";
                    return RedirectToAction("Index", new { tenantId = tenantId });
                }

                // Solo AdminPlataforma puede cambiar el tenant
                if (User.IsInRole("AdminPlataforma"))
                {
                    ViewBag.Tenants = await _iTenantService.GetAllAsync();
                }

                return View(audiencia);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar audiencia para editar: {ex.Message}";
                Console.WriteLine($"❌ Error en Editar GET: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Guid id, AudienciaDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Guid tenantId = GetEffectiveTenantId();
                    
                    // Establecer el ID para actualización
                    dto.Id = id;

                    await _iAudienciaService.GuardarAudienciaAsync(tenantId, dto);
                    
                    TempData["Success"] = "Audiencia actualizada exitosamente.";
                    return RedirectToAction("Index", new { tenantId = tenantId });
                }

                // Si hay errores de validación, recargar datos necesarios
                if (User.IsInRole("AdminPlataforma"))
                {
                    ViewBag.Tenants = await _iTenantService.GetAllAsync();
                }

                return View(dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"Error de operación: {ex.Message}";
                ModelState.AddModelError("", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = $"Audiencia no encontrada: {ex.Message}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar audiencia: {ex.Message}";
                Console.WriteLine($"❌ Error en Editar POST: {ex.Message}");
            }

            // Recargar datos en caso de error
            if (User.IsInRole("AdminPlataforma"))
            {
                ViewBag.Tenants = await _iTenantService.GetAllAsync();
            }
            
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(Guid id)
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();
                
                // Obtener la audiencia específica
                var audiencias = await _iAudienciaService.GetDefinicionesAudienciaAsync(tenantId);
                var audiencia = audiencias.FirstOrDefault(a => a.Id == id);

                if (audiencia == null)
                {
                    TempData["Error"] = "Audiencia no encontrada.";
                    return RedirectToAction("Index", new { tenantId = tenantId });
                }

                // Obtener información del tenant
                var tenant = await _iTenantService.GetByIdAsync(tenantId);
                ViewBag.TenantNombre = tenant?.Nombre ?? "Tenant no encontrado";

                // Obtener usuarios afectados
                var usuarios = await _iAudienciaService.GetUsuariosPorAudienciaAsync(tenantId, audiencia.NombreUnicoInterno);
                ViewBag.UsuariosAfectados = usuarios.Count();

                return View(audiencia);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar audiencia para eliminar: {ex.Message}";
                Console.WriteLine($"❌ Error en Borrar GET: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Borrar")]
        public async Task<IActionResult> BorrarConfirmed(Guid id)
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();
                
                await _iAudienciaService.EliminarAudienciaAsync(tenantId, id);
                
                TempData["Success"] = "Audiencia eliminada exitosamente.";
                return RedirectToAction("Index", new { tenantId = tenantId });
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = $"Audiencia no encontrada: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar audiencia: {ex.Message}";
                Console.WriteLine($"❌ Error en BorrarConfirmed: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            try
            {
                // Solo AdminPlataforma puede seleccionar tenant, AdminTenant usa el suyo
                if (User.IsInRole("AdminPlataforma"))
                {
                    ViewBag.Tenants = await _iTenantService.GetAllAsync();
                }
                
                return View(new AudienciaDto());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar formulario de creación: {ex.Message}";
                Console.WriteLine($"❌ Error en Crear GET: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear(AudienciaDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Guid tenantId = GetEffectiveTenantId();

                    // Limpiar el ID para creación
                    dto.Id = Guid.Empty;

                    await _iAudienciaService.GuardarAudienciaAsync(tenantId, dto);
                    
                    TempData["Success"] = "Audiencia creada exitosamente.";
                    return RedirectToAction("Index", new { tenantId = tenantId });
                }

                // Si hay errores de validación, recargar tenants si es AdminPlataforma
                if (User.IsInRole("AdminPlataforma"))
                {
                    ViewBag.Tenants = await _iTenantService.GetAllAsync();
                }
                
                return View(dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"Error de operación: {ex.Message}";
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear audiencia: {ex.Message}";
                Console.WriteLine($"❌ Error en Crear POST: {ex.Message}");
            }

            // Recargar datos en caso de error
            if (User.IsInRole("AdminPlataforma"))
            {
                ViewBag.Tenants = await _iTenantService.GetAllAsync();
            }
            
            return View(dto);
        }

        /// <summary>
        /// Recalcula los segmentos de usuarios para el tenant
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RecalcularSegmentos()
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();

                await _iAudienciaService.ActualizarSegmentosUsuariosAsync(tenantId, null);
                
                TempData["Success"] = "Recalculación de segmentos iniciada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al recalcular segmentos: {ex.Message}";
                Console.WriteLine($"❌ Error en RecalcularSegmentos: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Muestra estadísticas globales de audiencias
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Estadisticas()
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();

                // Obtener estadísticas globales
                var estadisticas = await _iAudienciaService.GetEstadisticasGlobalesYAporAudienciaAsync(tenantId);
                ViewBag.Estadisticas = estadisticas;

                // Obtener distribución de usuarios
                var distribucion = await _iAudienciaService.GetDistribucionUsuariosPorAudienciaAsync(tenantId);
                ViewBag.Distribucion = distribucion;

                // Obtener información del tenant
                var tenant = await _iTenantService.GetByIdAsync(tenantId);
                ViewBag.TenantNombre = tenant?.Nombre ?? "Tenant no encontrado";

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar estadísticas: {ex.Message}";
                Console.WriteLine($"❌ Error en Estadisticas: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Clasifica un usuario específico (AJAX)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ClasificarUsuario(Guid usuarioId)
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();

                string nombreUnicoAudiencia = await _iAudienciaService.ClasificarUsuarioAsync(usuarioId, tenantId);
                
                return Json(new { 
                    success = true, 
                    usuarioId = usuarioId, 
                    audienciaAsignada = nombreUnicoAudiencia ?? "Sin audiencia específica"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = $"Usuario no encontrado: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ClasificarUsuario: {ex.Message}");
                return Json(new { success = false, message = "Error interno al clasificar usuario" });
            }
        }

        /// <summary>
        /// Obtiene usuarios de una audiencia específica (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsuariosAudiencia(string nombreUnicoAudiencia)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreUnicoAudiencia))
                {
                    return Json(new { success = false, message = "Nombre de audiencia requerido" });
                }

                Guid tenantId = GetEffectiveTenantId();

                var usuarios = await _iAudienciaService.GetUsuariosPorAudienciaAsync(tenantId, nombreUnicoAudiencia);
                
                var usuariosDto = usuarios.Select(u => new {
                    id = u.Id,
                    nombre = u.Nombre,
                    email = u.Email,
                    puntos = u.Puntos
                });

                return Json(new { success = true, usuarios = usuariosDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetUsuariosAudiencia: {ex.Message}");
                return Json(new { success = false, message = "Error al obtener usuarios" });
            }
        }

        /// <summary>
        /// Obtiene la distribución de audiencias (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDistribucion()
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();

                var distribucion = await _iAudienciaService.GetDistribucionUsuariosPorAudienciaAsync(tenantId);
                
                return Json(new { success = true, distribucion = distribucion });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetDistribucion: {ex.Message}");
                return Json(new { success = false, message = "Error al obtener distribución" });
            }
        }
<<<<<<< HEAD
=======

        /// <summary>
        /// Obtiene todos los usuarios del tenant actual (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsuariosTenant()
        {
            try
            {
                Guid tenantId = GetEffectiveTenantId();
                var usuarios = await _iUsuarioService.GetAllUsuariosAsync(tenantId);
                var usuariosDto = usuarios.Select(u => new { id = u.Id, nombre = u.Nombre });
                return Json(new { success = true, usuarios = usuariosDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetUsuariosTenant: {ex.Message}");
                return Json(new { success = false, message = "Error al obtener usuarios" });
            }
        }
>>>>>>> origin/dev
    }
}