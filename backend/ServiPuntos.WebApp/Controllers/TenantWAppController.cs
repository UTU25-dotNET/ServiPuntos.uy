using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers
{
    [Authorize(Roles = "AdminTenant, AdminPlataforma")]
    public class TenantWAppController : Controller
    {
        private readonly ITenantService _iTenantService;

        public TenantWAppController(ITenantService tenantService)
        {
            _iTenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenants = await _iTenantService.GetAllAsync();
            return View(tenants);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Guid id, string nombre, string logoUrl, string color)
        {
            // *** DEBUG: Log de valores recibidos ***
            Console.WriteLine("=== DEBUG EDITAR TENANT ===");
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"Nombre: '{nombre}'");
            Console.WriteLine($"LogoUrl: '{logoUrl}'");
            Console.WriteLine($"Color: '{color}'");
            Console.WriteLine("============================");

            if (ModelState.IsValid)
            {
                var tenant = await _iTenantService.GetByIdAsync(id);
                if (tenant == null)
                {
                    Console.WriteLine($"❌ Tenant con ID {id} no encontrado");
                    return NotFound();
                }

                Console.WriteLine($"🔍 Tenant encontrado - Color actual en BD: '{tenant.Color}'");

                // Actualizar propiedades del tenant
                var colorAnterior = tenant.Color;
                tenant.Nombre = nombre;
                tenant.LogoUrl = logoUrl ?? string.Empty;
                tenant.Color = color ?? string.Empty;
                tenant.FechaModificacion = DateTime.UtcNow;

                Console.WriteLine($"🔄 Actualizando tenant:");
                Console.WriteLine($"   Nombre: '{colorAnterior}' → '{tenant.Color}'");
                Console.WriteLine($"   Color: '{colorAnterior}' → '{tenant.Color}'");

                try
                {
                    await _iTenantService.UpdateAsync(tenant);
                    Console.WriteLine("✅ Tenant actualizado correctamente en servicio");

                    // Verificar que se guardó
                    var tenantVerificacion = await _iTenantService.GetByIdAsync(id);
                    Console.WriteLine($"🔍 Verificación - Color en BD después de guardar: '{tenantVerificacion.Color}'");

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al actualizar tenant: {ex.Message}");
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("❌ ModelState no válido:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"   {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            var tenantModel = await _iTenantService.GetByIdAsync(id);
            return View(tenantModel);
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        [HttpPost, ActionName("Borrar")]
        public async Task<IActionResult> BorrarConfirmed(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            if (tenant != null)
            {
                await _iTenantService.DeleteAsync(id);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre, string logoUrl, string color)
        {
            // *** DEBUG: Log de valores recibidos ***
            Console.WriteLine("=== DEBUG CREAR TENANT ===");
            Console.WriteLine($"Nombre: '{nombre}'");
            Console.WriteLine($"LogoUrl: '{logoUrl}'");
            Console.WriteLine($"Color: '{color}'");
            Console.WriteLine("===========================");

            if (ModelState.IsValid)
            {
                var tenant = new Tenant 
                {
                    Id = Guid.NewGuid(),
                    Nombre = nombre,
                    LogoUrl = logoUrl ?? string.Empty,
                    Color = color ?? string.Empty,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow
                };

                Console.WriteLine($"🆕 Creando tenant con color: '{tenant.Color}'");

                try
                {
                    await _iTenantService.AddAsync(tenant);
                    Console.WriteLine("✅ Tenant creado correctamente");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al crear tenant: {ex.Message}");
                    ModelState.AddModelError("", $"Error al crear: {ex.Message}");
                }
            }
            return View();
        }

        // *** Método de diagnóstico adicional ***
        [HttpGet]
        public async Task<IActionResult> DiagnosticarColor(Guid id)
        {
            var tenant = await _iTenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound();

            var diagnostico = new
            {
                TenantId = tenant.Id,
                ColorEnBD = tenant.Color,
                ColorVacio = string.IsNullOrEmpty(tenant.Color),
                ColorLength = tenant.Color?.Length ?? 0,
                ColorBytes = tenant.Color != null ? Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(tenant.Color)) : "NULL"
            };

            return Json(diagnostico);
        }
    }
}