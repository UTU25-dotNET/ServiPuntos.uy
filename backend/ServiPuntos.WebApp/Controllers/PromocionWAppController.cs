using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.WebApp.Models;
using System.Threading.Tasks;

namespace ServiPuntos.WebApp.Controllers
{
    [Authorize(Roles = "AdminTenant")]
    public class PromocionWAppController : Controller
    {
        private readonly IPromocionService _promocionService;
        private readonly IUbicacionService _ubicacionService;
        private readonly IProductoCanjeableService _productoService;
        private readonly IAudienciaService _audienciaService;
        private readonly INotificacionService _notificacionService;
        private readonly ITenantContext _tenantContext;

        public PromocionWAppController(
            IPromocionService promocionService,
            IUbicacionService ubicacionService,
            IProductoCanjeableService productoService,
            IAudienciaService audienciaService,
            INotificacionService notificacionService,
            ITenantContext tenantContext)
        {
            _promocionService = promocionService;
            _ubicacionService = ubicacionService;
            _productoService = productoService;
            _audienciaService = audienciaService;
            _notificacionService = notificacionService;
            _tenantContext = tenantContext;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();

            var promos = await _promocionService.GetPromocionesByTenantAsync(tenantId);
            return View(promos);
        }

        [HttpGet]
        public async Task<IActionResult> CrearPromocion()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            ViewBag.Productos = await _productoService.GetAllProductosAsync();
            ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
            return View("CrearPromocion", new CreatePromocionViewModel { Tipo = TipoPromocion.Promocion });
        }

        [HttpGet]
        public async Task<IActionResult> CrearOferta()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            ViewBag.Productos = new List<ProductoCanjeable>();
            ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
            return View("CrearOferta", new CreatePromocionViewModel { Tipo = TipoPromocion.Oferta });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPromocion(CreatePromocionViewModel model)
        {
            return await CrearInterno(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearOferta(CreatePromocionViewModel model)
        {
            model.Tipo = TipoPromocion.Oferta;
            return await CrearInterno(model);
        }

        private async Task<IActionResult> CrearInterno(CreatePromocionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            if (!ModelState.IsValid)
            {
                ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                ViewBag.Productos = model.Tipo == TipoPromocion.Oferta
                    ? new List<ProductoCanjeable>()
                    : await _productoService.GetAllProductosAsync();
                ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
                return View(model.Tipo == TipoPromocion.Oferta ? "CrearOferta" : "CrearPromocion", model);
            }

            var ubicaciones = new List<Ubicacion>();
            foreach (var uid in model.UbicacionIds)
            {
                var ub = await _ubicacionService.GetUbicacionAsync(uid);
                if (ub != null) ubicaciones.Add(ub);
            }

            var fechaInicio = DateTime.SpecifyKind(model.FechaInicio, DateTimeKind.Utc);
            var fechaFin = DateTime.SpecifyKind(model.FechaFin, DateTimeKind.Utc);

            decimal? descuento = model.Tipo == TipoPromocion.Promocion ? null : model.DescuentoEnPesos;
            int? precioPuntos = model.Tipo == TipoPromocion.Oferta ? null : model.PrecioEnPuntos;
            decimal? precioPesos = model.PrecioEnPesos;

            var productos = new List<PromocionProducto>();
            foreach (var pid in model.ProductoIds)
            {
                var prod = await _productoService.GetProductoAsync(pid);
                if (prod != null)
                {
                    productos.Add(new PromocionProducto { ProductoCanjeableId = pid, ProductoCanjeable = prod });
                }
            }

            var promo = new Promocion
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                DescuentoEnPesos = descuento,
                PrecioEnPuntos = precioPuntos,
                PrecioEnPesos = precioPesos,
                Tipo = model.Tipo,
                TenantId = tenantId,
                AudienciaId = model.AudienciaId,
                Ubicaciones = ubicaciones,
                Productos = productos
            };
            await _promocionService.AddPromocionAsync(promo);
            TempData["Success"] = "Promoción creada";
            return RedirectToAction(nameof(Index), nameof(PromocionWAppController).Replace("Controller", ""));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();
            var model = new EditPromocionViewModel
            {
                Id = promo.Id,
                Titulo = promo.Titulo,
                Descripcion = promo.Descripcion,
                PrecioEnPuntos = promo.PrecioEnPuntos,
                PrecioEnPesos = promo.PrecioEnPesos,
                DescuentoEnPesos = promo.DescuentoEnPesos,
                FechaInicio = promo.FechaInicio,
                FechaFin = promo.FechaFin,
                Tipo = promo.Tipo,
                AudienciaId = promo.AudienciaId,
                UbicacionIds = promo.Ubicaciones?.Select(u => u.Id).ToList() ?? new List<Guid>(),
                ProductoIds = promo.Productos?.Select(pp => pp.ProductoCanjeableId).ToList() ?? new List<Guid>()
            };
            ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
            ViewBag.Productos = await _productoService.GetAllProductosAsync();
            ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, EditPromocionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                ViewBag.Productos = await _productoService.GetAllProductosAsync();
                ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
                return View(model);
            }

            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();

            // Ensure UTC dates for PostgreSQL
            promo.FechaInicio = DateTime.SpecifyKind(model.FechaInicio, DateTimeKind.Utc);
            promo.FechaFin = DateTime.SpecifyKind(model.FechaFin, DateTimeKind.Utc);

            // Enforce business rules depending on the tipo de promoción
            promo.DescuentoEnPesos = model.Tipo == TipoPromocion.Promocion ? null : model.DescuentoEnPesos;
            promo.PrecioEnPuntos = model.Tipo == TipoPromocion.Oferta ? null : model.PrecioEnPuntos;
            promo.PrecioEnPesos = model.PrecioEnPesos;

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
            var pps = new List<PromocionProducto>();
            foreach (var pid in model.ProductoIds)
            {
                var prod = await _productoService.GetProductoAsync(pid);
                if (prod != null)
                {
                    pps.Add(new PromocionProducto { ProductoCanjeableId = pid, ProductoCanjeable = prod });
                }
            }
            promo.Productos = pps;

            await _promocionService.UpdatePromocionAsync(promo);
            TempData["Success"] = "Promoción actualizada";
            return RedirectToAction(nameof(Index), nameof(PromocionWAppController).Replace("Controller", ""));
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();
            return View(promo);
        }

        [HttpPost, ActionName("Borrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarConfirmed(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();

            await _promocionService.DeletePromocionAsync(id);
            TempData["Success"] = "Promoción eliminada";
            return RedirectToAction(nameof(Index), nameof(PromocionWAppController).Replace("Controller", ""));
        }

        [HttpGet]
        public async Task<IActionResult> Notificar(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();

            ViewBag.Promocion = promo;
            ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);

            var model = new CreateNotificacionViewModel
            {
                Titulo = $"{promo.Titulo}",
                Mensaje = "Nueva promoción disponible"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notificar(Guid id, CreateNotificacionViewModel model)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null || promo.TenantId != tenantId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Promocion = promo;
                ViewBag.Audiencias = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
                return View(model);
            }

            var notif = new Notificacion
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                AudienciaId = model.AudienciaId,
                Titulo = model.Titulo,
                Mensaje = model.Mensaje,
                FechaCreacion = DateTime.UtcNow
            };

            await _notificacionService.CrearNotificacionAsync(notif, model.AudienciaId);

            TempData["Success"] = "Notificación enviada";
            return RedirectToAction(nameof(Index));
        }
    }
}