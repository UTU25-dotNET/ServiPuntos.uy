using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Ajusta según tu sistema de autenticación
    public class ProductoCanjeableController : ControllerBase
    {
        private readonly IProductoCanjeableService _productoCanjeableService;
        private readonly IProductoUbicacionService _productoUbicacionService;
        private readonly IUbicacionService _ubicacionService;
        private readonly ITenantContext _tenantContext;

        public ProductoCanjeableController(
            IProductoCanjeableService productoCanjeableService,
            IProductoUbicacionService productoUbicacionService,
            IUbicacionService ubicacionService,
            ITenantContext tenantContext)
        {
            _productoCanjeableService = productoCanjeableService;
            _productoUbicacionService = productoUbicacionService;
            _ubicacionService = ubicacionService;
            _tenantContext = tenantContext;
        }

        /// <summary>
        /// Obtiene todos los productos canjeables con sus ubicaciones
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetProductosCanjeables()
        {
            try
            {
                var tenantId = _tenantContext.TenantId;
                if (tenantId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Tenant no válido" });
                }

                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                var productos = await _productoCanjeableService.GetAllProductosAsync();

                var response = new
                {
                    productos = productos.Select(p => new
                    {
                        id = p.Id,
                        nombre = p.Nombre,
                        descripcion = p.Descripcion,
                        costoEnPuntos = p.CostoEnPuntos,
                        fotoUrl = p.FotoUrl
                    }).ToList(),
                    ubicaciones = ubicaciones.Select(u => new
                    {
                        id = u.Id,
                        nombre = u.Nombre,
                        tenantId = u.TenantId
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto canjeable por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProductoCanjeable(Guid id)
        {
            try
            {
                var producto = await _productoCanjeableService.GetProductoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                var response = new
                {
                    id = producto.Id,
                    nombre = producto.Nombre,
                    descripcion = producto.Descripcion,
                    costoEnPuntos = producto.CostoEnPuntos,
                    fotoUrl = producto.FotoUrl
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto canjeable
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> CreateProductoCanjeable([FromBody] CreateProductoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new { message = "Datos inválidos", errors });
                }

                var producto = new ProductoCanjeable(
                    request.Nombre,
                    request.Descripcion,
                    (int)request.CostoEnPuntos
                )
                {
                    FotoUrl = request.FotoUrl
                };

                await _productoCanjeableService.AddProductoAsync(producto);

                var response = new
                {
                    id = producto.Id,
                    nombre = producto.Nombre,
                    descripcion = producto.Descripcion,
                    costoEnPuntos = producto.CostoEnPuntos,
                    fotoUrl = producto.FotoUrl
                };

                return CreatedAtAction(nameof(GetProductoCanjeable), new { id = producto.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto canjeable
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateProductoCanjeable(Guid id, [FromBody] UpdateProductoRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new { message = "El ID no coincide" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(new { message = "Datos inválidos", errors });
                }

                var producto = await _productoCanjeableService.GetProductoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                producto.Nombre = request.Nombre;
                producto.Descripcion = request.Descripcion;
                producto.CostoEnPuntos = (int)request.CostoEnPuntos;
                producto.FotoUrl = request.FotoUrl;

                await _productoCanjeableService.UpdateProductoAsync(producto);

                var response = new
                {
                    id = producto.Id,
                    nombre = producto.Nombre,
                    descripcion = producto.Descripcion,
                    costoEnPuntos = producto.CostoEnPuntos,
                    fotoUrl = producto.FotoUrl
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un producto canjeable
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteProductoCanjeable(Guid id)
        {
            try
            {
                var producto = await _productoCanjeableService.GetProductoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                await _productoCanjeableService.DeleteProductoAsync(id);

                return Ok(new { message = "Producto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Asigna un producto a ubicaciones específicas
        /// </summary>
        [HttpPost("{id}/asignar-ubicaciones")]
        public async Task<ActionResult<object>> AsignarProductoUbicaciones(Guid id, [FromBody] AsignarUbicacionesRequest request)
        {
            try
            {
                var tenantId = _tenantContext.TenantId;
                if (tenantId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Tenant no válido" });
                }

                var producto = await _productoCanjeableService.GetProductoAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                var ubicacionesAsignadas = 0;
                foreach (var ubicacionId in request.UbicacionIds)
                {
                    var productoUbicacion = new ProductoUbicacion(
                        ubicacionId,
                        id,
                        request.StockInicial
                    )
                    {
                        Activo = true
                    };

                    await _productoUbicacionService.AddAsync(productoUbicacion);
                    ubicacionesAsignadas++;
                }

                return Ok(new { message = $"Producto asignado a {ubicacionesAsignadas} ubicaciones exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el stock de productos por ubicación
        /// </summary>
        [HttpGet("ubicacion/{ubicacionId}/stock")]
        public async Task<ActionResult<object>> GetStockPorUbicacion(Guid ubicacionId)
        {
            try
            {
                var tenantId = _tenantContext.TenantId;
                if (tenantId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Tenant no válido" });
                }

                var ubicacion = await _ubicacionService.GetUbicacionAsync(ubicacionId);
                if (ubicacion == null || ubicacion.TenantId != tenantId)
                {
                    return NotFound(new { message = "Ubicación no encontrada" });
                }

                var productosUbicacion = await _productoUbicacionService.GetAllAsync(ubicacionId);

                var response = new
                {
                    ubicacionId = ubicacionId,
                    ubicacionNombre = ubicacion.Nombre,
                    productosStock = productosUbicacion.Select(pu => new
                    {
                        id = pu.Id,
                        productoId = pu.ProductoCanjeableId,
                        productoNombre = pu.ProductoCanjeable?.Nombre ?? "",
                        stockDisponible = pu.StockDisponible,
                        activo = pu.Activo
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto en una ubicación
        /// </summary>
        [HttpPut("stock/{productoUbicacionId}")]
        public async Task<ActionResult<object>> ActualizarStock(Guid productoUbicacionId, [FromBody] ActualizarStockRequest request)
        {
            try
            {
                var productoUbicacion = await _productoUbicacionService.GetAsync(productoUbicacionId);
                if (productoUbicacion == null)
                {
                    return NotFound(new { message = "Producto ubicación no encontrado" });
                }

                productoUbicacion.StockDisponible = request.NuevoStock;
                productoUbicacion.Activo = request.Activo;

                await _productoUbicacionService.UpdateAsync(productoUbicacion);

                return Ok(new { message = "Stock actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }

    // ===============================
    // Request Models simplificados
    // ===============================

    public class CreateProductoRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El costo en puntos es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        public decimal CostoEnPuntos { get; set; }

        public string? FotoUrl { get; set; }
    }

    public class UpdateProductoRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El costo en puntos es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        public decimal CostoEnPuntos { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class AsignarUbicacionesRequest
    {
        [Required(ErrorMessage = "Debe seleccionar al menos una ubicación")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos una ubicación")]
        public List<Guid> UbicacionIds { get; set; } = new List<Guid>();

        [Required(ErrorMessage = "El stock inicial es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock inicial debe ser mayor o igual a 0")]
        public int StockInicial { get; set; } = 10;
    }

    public class ActualizarStockRequest
    {
        [Required(ErrorMessage = "El nuevo stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int NuevoStock { get; set; }

        public bool Activo { get; set; } = true;
    }
}