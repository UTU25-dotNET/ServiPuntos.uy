using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoUbicacionController : ControllerBase
    {
        private readonly IProductoUbicacionService _productoUbicacionService;

        public ProductoUbicacionController(IProductoUbicacionService productoUbicacionService)
        {
            _productoUbicacionService = productoUbicacionService;
        }

        /// <summary>
        /// Obtiene todos los productos por ubicación
        /// </summary>
        /// <returns>Lista de productos por ubicación</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoUbicacion>>> GetAll()
        {
            try
            {
                var productosUbicacion = await _productoUbicacionService.GetAllAsync();
                return Ok(productosUbicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los productos de una ubicación específica
        /// </summary>
        /// <param name="ubicacionId">ID de la ubicación</param>
        /// <returns>Lista de productos de la ubicación</returns>
        [HttpGet("ubicacion/{ubicacionId}")]
        public async Task<ActionResult<IEnumerable<ProductoUbicacion>>> GetAllByUbicacion(Guid ubicacionId)
        {
            try
            {
                if (ubicacionId == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID de ubicación no puede estar vacío" });
                }

                var productosUbicacion = await _productoUbicacionService.GetAllAsync(ubicacionId);
                return Ok(productosUbicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un producto ubicación por su ID
        /// </summary>
        /// <param name="id">ID del producto ubicación</param>
        /// <returns>Producto ubicación encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoUbicacion>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID no puede estar vacío" });
                }

                var productoUbicacion = await _productoUbicacionService.GetAsync(id);

                if (productoUbicacion == null)
                {
                    return NotFound(new { message = "Producto ubicación no encontrado" });
                }

                return Ok(productoUbicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo producto ubicación
        /// </summary>
        /// <param name="request">Datos del producto ubicación a crear</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductoUbicacionDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productoUbicacion = new ProductoUbicacion(
                    request.UbicacionId,
                    request.ProductoCanjeableId,
                    request.StockDisponible)
                {
                    Id = Guid.NewGuid(),
                    Activo = request.Activo
                };

                await _productoUbicacionService.AddAsync(productoUbicacion);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = productoUbicacion.Id },
                    productoUbicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un producto ubicación existente
        /// </summary>
        /// <param name="id">ID del producto ubicación a actualizar</param>
        /// <param name="request">Datos actualizados</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] ProductoUbicacionDto request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID no puede estar vacío" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productoUbicacionExistente = await _productoUbicacionService.GetAsync(id);

                if (productoUbicacionExistente == null)
                {
                    return NotFound(new { message = "Producto ubicación no encontrado" });
                }

                // Actualizar propiedades
                productoUbicacionExistente.UbicacionId = request.UbicacionId;
                productoUbicacionExistente.ProductoCanjeableId = request.ProductoCanjeableId;
                productoUbicacionExistente.StockDisponible = request.StockDisponible;
                productoUbicacionExistente.Activo = request.Activo;

                await _productoUbicacionService.UpdateAsync(productoUbicacionExistente);

                return Ok(new { message = "Producto ubicación actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza solo el stock de un producto ubicación
        /// </summary>
        /// <param name="id">ID del producto ubicación</param>
        /// <param name="request">Nuevo stock</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult> UpdateStock(Guid id, [FromBody] ProductoUbicacionDto request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID no puede estar vacío" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productoUbicacion = await _productoUbicacionService.GetAsync(id);

                if (productoUbicacion == null)
                {
                    return NotFound(new { message = "Producto ubicación no encontrado" });
                }

                productoUbicacion.StockDisponible = request.StockDisponible;
                await _productoUbicacionService.UpdateAsync(productoUbicacion);

                return Ok(new { message = "Stock actualizado correctamente", nuevoStock = request.StockDisponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un producto ubicación
        /// </summary>
        /// <param name="id">ID del producto ubicación</param>
        /// <param name="request">Estado activo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/estado")]
        public async Task<ActionResult> UpdateEstado(Guid id, [FromBody] ProductoUbicacionDto request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID no puede estar vacío" });
                }

                var productoUbicacion = await _productoUbicacionService.GetAsync(id);

                if (productoUbicacion == null)
                {
                    return NotFound(new { message = "Producto ubicación no encontrado" });
                }

                productoUbicacion.Activo = request.Activo;
                await _productoUbicacionService.UpdateAsync(productoUbicacion);

                string estado = request.Activo ? "activado" : "desactivado";
                return Ok(new { message = $"Producto ubicación {estado} correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
