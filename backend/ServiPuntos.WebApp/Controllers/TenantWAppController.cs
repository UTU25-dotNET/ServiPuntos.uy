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

        public async Task<IActionResult> IndexAsync()
        {
            var tenants = await _iTenantService.GetAllAsync();
            return View(tenants);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string nombre)
        {
            if (ModelState.IsValid)
            {
                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Nombre = nombre,
                    LogoUrl = string.Empty,
                    Color = string.Empty,
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow
                };

                await _iTenantService.AddAsync(tenant);
                return RedirectToAction("Index");
            }
            return View();
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
        public async Task<IActionResult> Editar(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                tenant.FechaModificacion = DateTime.UtcNow;
                await _iTenantService.UpdateAsync(tenant);
                return RedirectToAction("Index");
            }
            return View(tenant);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarConfirmado(Guid id)
        {
            await _iTenantService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
