using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromocionController : ControllerBase
    {
        private readonly IPromocionService _promocionService;
        private readonly IUbicacionRepository _ubicacionRepository;
        private readonly IProductoCanjeableService _productoService;
        private readonly ITenantContext _tenantContext;

        public PromocionController(IPromocionService promocionService, IUbicacionRepository ubicacionRepository, IProductoCanjeableService productoService, ITenantContext tenantContext)
        {
            _promocionService = promocionService;
            _ubicacionRepository = ubicacionRepository;
            _productoService = productoService;
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> Get()
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized(new { message = "Tenant no válido" });
            var promos = await _promocionService.GetPromocionesByTenantAsync(tenantId);
            var result = promos.Select(p => new
            {
                id = p.Id,
                titulo = p.Titulo,
                descripcion = p.Descripcion,
                fechaInicio = p.FechaInicio,
                fechaFin = p.FechaFin,
                tipo = p.Tipo.ToString(),
                precioEnPuntos = p.PrecioEnPuntos,
                precioEnPesos = p.PrecioEnPesos,
                descuentoEnPuntos = p.DescuentoEnPuntos,
                productoIds = p.Productos?.Select(pp => pp.ProductoCanjeableId).ToList()
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(Guid id)
        {
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null) return NotFound(new { message = "Promoción no encontrada" });
            var result = new
            {
                id = promo.Id,
                titulo = promo.Titulo,
                descripcion = promo.Descripcion,
                fechaInicio = promo.FechaInicio,
                fechaFin = promo.FechaFin,
                tipo = promo.Tipo.ToString(),
                precioEnPuntos = promo.PrecioEnPuntos,
                precioEnPesos = promo.PrecioEnPesos,
                descuentoEnPuntos = promo.DescuentoEnPuntos,
                ubicaciones = promo.Ubicaciones?.Select(u => u.Id).ToList(),
                audienciaId = promo.AudienciaId,
                productoIds = promo.Productos?.Select(pp => pp.ProductoCanjeableId).ToList()
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreatePromocionRequest request)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == null) return Unauthorized(new { message = "Tenant no válido" });
            var ubicaciones = new List<Ubicacion>();
            if (request.UbicacionIds != null && request.UbicacionIds.Count > 0)
            {
                foreach (var id in request.UbicacionIds)
                {
                    var ub = await _ubicacionRepository.GetAsync(id);
                    if (ub != null) ubicaciones.Add(ub);
                }
            }

            var productos = new List<PromocionProducto>();
            if (request.ProductoIds != null)
            {
                foreach (var pid in request.ProductoIds)
                {
                    var prod = await _productoService.GetProductoAsync(pid);
                    if (prod != null)
                    {
                        productos.Add(new PromocionProducto
                        {
                            ProductoCanjeableId = pid,
                            ProductoCanjeable = prod
                        });
                    }
                }
            }
            // Ensure UTC dates for PostgreSQL
            var fechaInicio = DateTime.SpecifyKind(request.FechaInicio, DateTimeKind.Utc);
            var fechaFin = DateTime.SpecifyKind(request.FechaFin, DateTimeKind.Utc);

            // Enforce business rules depending on the tipo de promoción
            int? descuento = request.Tipo == Core.Enums.TipoPromocion.Promocion ? null : request.DescuentoEnPuntos;
            int? precioPuntos = request.Tipo == Core.Enums.TipoPromocion.Oferta ? null : request.PrecioEnPuntos;
            decimal? precioPesos = request.Tipo == Core.Enums.TipoPromocion.Oferta ? null : request.PrecioEnPesos;

            var promo = new Promocion
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                DescuentoEnPuntos = descuento,
                PrecioEnPuntos = precioPuntos,
                PrecioEnPesos = precioPesos,
                Tipo = request.Tipo,
                TenantId = tenantId,
                AudienciaId = request.AudienciaId,
                Ubicaciones = ubicaciones,
                Productos = productos
            };
            await _promocionService.AddPromocionAsync(promo);
            return CreatedAtAction(nameof(GetById), new { id = promo.Id }, new { id = promo.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Update(Guid id, [FromBody] CreatePromocionRequest request)
        {
            var promo = await _promocionService.GetPromocionAsync(id);
            if (promo == null) return NotFound(new { message = "Promoción no encontrada" });

            // Ensure UTC dates for PostgreSQL
            promo.FechaInicio = DateTime.SpecifyKind(request.FechaInicio, DateTimeKind.Utc);
            promo.FechaFin = DateTime.SpecifyKind(request.FechaFin, DateTimeKind.Utc);

            // Enforce business rules depending on the tipo de promoción
            promo.DescuentoEnPuntos = request.Tipo == Core.Enums.TipoPromocion.Promocion ? null : request.DescuentoEnPuntos;
            promo.PrecioEnPuntos = request.Tipo == Core.Enums.TipoPromocion.Oferta ? null : request.PrecioEnPuntos;
            promo.PrecioEnPesos = request.Tipo == Core.Enums.TipoPromocion.Oferta ? null : request.PrecioEnPesos;

            promo.Titulo = request.Titulo;
            promo.Descripcion = request.Descripcion;
            promo.Tipo = request.Tipo;
            promo.AudienciaId = request.AudienciaId;
            if (request.UbicacionIds != null)
            {
                var ubs = new List<Ubicacion>();
                foreach (var uid in request.UbicacionIds)
                {
                    var ub = await _ubicacionRepository.GetAsync(uid);
                    if (ub != null) ubs.Add(ub);
                }
                promo.Ubicaciones = ubs;
            }
            if (request.ProductoIds != null)
            {
                var pps = new List<PromocionProducto>();
                foreach (var pid in request.ProductoIds)
                {
                    var prod = await _productoService.GetProductoAsync(pid);
                    if (prod != null)
                    {
                        pps.Add(new PromocionProducto
                        {
                            ProductoCanjeableId = pid,
                            ProductoCanjeable = prod
                        });
                    }
                }
                promo.Productos = pps;
            }
            await _promocionService.UpdatePromocionAsync(promo);
            return Ok(new { id = promo.Id });
        }
    }

    public class CreatePromocionRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? DescuentoEnPuntos { get; set; }
        public int? PrecioEnPuntos { get; set; }
        public decimal? PrecioEnPesos { get; set; }
        public Core.Enums.TipoPromocion Tipo { get; set; } = Core.Enums.TipoPromocion.Promocion;
        public Guid? AudienciaId { get; set; }
        public List<Guid>? UbicacionIds { get; set; }
        public List<Guid>? ProductoIds { get; set; }
    }
}