using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.WebApp.Models;

namespace ServiPuntos.WebApp.Controllers
{
    [Authorize(Roles = "AdminTenant")]
    public class PromocionWAppController : Controller
    {
        private readonly IPromocionService _promocionService;
        private readonly IUbicacionService _ubicacionService;
        private readonly ITenantContext _tenantContext;

        public PromocionWAppController(
            IPromocionService promocionService,
            IUbicacionService ubicacionService,
            ITenantContext tenantContext)
        {
            _promocionService = promocionService;
            _ubicacionService = ubicacionService;
            _tenantContext = tenantContext;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized();

            var promos = await _promocionService.GetPromocionesByTenantAsync(tenantId);
            return View(promos);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized();
            ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            return View(new CreatePromocionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CreatePromocionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized();
            if (!ModelState.IsValid)
            {
                ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                return View(model);
            }

            var ubicaciones = new List<Ubicacion>();
            foreach (var uid in model.UbicacionIds)
            {
                var ub = await _ubicacionService.GetUbicacionAsync(uid);
                if (ub != null) ubicaciones.Add(ub);
            }

            // Ensure UTC dates for PostgreSQL
            var fechaInicio = DateTime.SpecifyKind(model.FechaInicio, DateTimeKind.Utc);
            var fechaFin = DateTime.SpecifyKind(model.FechaFin, DateTimeKind.Utc);

            // Enforce business rules depending on the tipo de promoción
            int? descuento = model.Tipo == TipoPromocion.Promocion ? null : model.DescuentoEnPuntos;
            int? precioPuntos = model.Tipo == TipoPromocion.Oferta ? null : model.PrecioEnPuntos;

            var promo = new Promocion
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                DescuentoEnPuntos = descuento,
                PrecioEnPuntos = precioPuntos,
                Tipo = model.Tipo,
                TenantId = tenantId,
                AudienciaId = model.AudienciaId,
                Ubicaciones = ubicaciones
            };
            await _promocionService.AddPromocionAsync(promo);
            TempData["Success"] = "Promoción creada";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();
            var model = new EditPromocionViewModel
            {
                Id = promo.Id,
                Titulo = promo.Titulo,
                Descripcion = promo.Descripcion,
                PrecioEnPuntos = promo.PrecioEnPuntos,
                DescuentoEnPuntos = promo.DescuentoEnPuntos,
                FechaInicio = promo.FechaInicio,
                FechaFin = promo.FechaFin,
                Tipo = promo.Tipo,
                AudienciaId = promo.AudienciaId,
                UbicacionIds = promo.Ubicaciones?.Select(u => u.Id).ToList() ?? new List<Guid>()
            };
            ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, EditPromocionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized();
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                return View(model);
            }

            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();

            // Ensure UTC dates for PostgreSQL
            promo.FechaInicio = DateTime.SpecifyKind(model.FechaInicio, DateTimeKind.Utc);
            promo.FechaFin = DateTime.SpecifyKind(model.FechaFin, DateTimeKind.Utc);

            // Enforce business rules depending on the tipo de promoción
            promo.DescuentoEnPuntos = model.Tipo == TipoPromocion.Promocion ? null : model.DescuentoEnPuntos;
            promo.PrecioEnPuntos = model.Tipo == TipoPromocion.Oferta ? null : model.PrecioEnPuntos;

            promo.Titulo = model.Titulo;
            promo.Descripcion = model.Descripcion;
            promo.Tipo = model.Tipo;
            promo.AudienciaId = model.AudienciaId;
            promo.Ubicaciones = new List<Ubicacion>();
            foreach (var uid in model.UbicacionIds)
            {
                var ub = await _ubicacionService.GetUbicacionAsync(uid);
                if (ub != null) promo.Ubicaciones.Add(ub);
            }

            await _promocionService.UpdatePromocionAsync(promo);
            TempData["Success"] = "Promoción actualizada";
            return RedirectToAction(nameof(Index));
        }
    }
}