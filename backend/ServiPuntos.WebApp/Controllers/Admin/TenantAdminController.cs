using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers.Admin
{
    public class TenantAdminController : Controller
    {
        private readonly ITenantService _tenantService;

        public TenantAdminController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _tenantService.GetAllAsync();
            return View(tenants); // ← carga la vista Index.cshtml
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound();
            return View(tenant);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tenant tenant)
        {
            if (!ModelState.IsValid) return View(tenant);

            tenant.Id = Guid.NewGuid();
            await _tenantService.AddAsync(tenant);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound();
            return View(tenant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Tenant updated)
        {
            if (!ModelState.IsValid) return View(updated);

            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound();

            tenant.Nombre = updated.Nombre;
            tenant.Color = updated.Color;
            tenant.LogoUrl = updated.LogoUrl;
            tenant.ValorPunto = updated.ValorPunto;

            await _tenantService.UpdateAsync(tenant);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            if (tenant == null) return NotFound();
            return View(tenant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _tenantService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
