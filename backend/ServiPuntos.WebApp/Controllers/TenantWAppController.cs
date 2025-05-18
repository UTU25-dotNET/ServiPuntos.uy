using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Controllers
{
    [Authorize (Roles = "AdminTenant")] // ver que es lo que va en la cookie
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
    }
}
