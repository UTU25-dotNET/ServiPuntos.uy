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
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            try
            {
                Console.WriteLine("[ProductoUbicacion] GetAll - Iniciando");
                var productosUbicacion = await _productoUbicacionService.GetAllAsync();

                var productosDto = productosUbicacion.Select(pu => new {
                    id = pu.Id,
                    ubicacionId = pu.UbicacionId,
                    categoria = pu.Categoria,
                    productoCanjeableId = pu.ProductoCanjeableId,
                    precio = pu.Precio,
                    stockDisponible = pu.StockDisponible,
                    activo = pu.Activo,
                    productoCanjeable = pu.ProductoCanjeable != null ? new {
                        id = pu.ProductoCanjeable.Id,
                        nombre = pu.ProductoCanjeable.Nombre,
                        descripcion = pu.ProductoCanjeable.Descripcion,
                        costoEnPuntos = pu.ProductoCanjeable.CostoEnPuntos,
                        fotoUrl = pu.ProductoCanjeable.FotoUrl
                    } : null
                }).ToList();

                Console.WriteLine($"[ProductoUbicacion] GetAll - Encontrados: {productosDto.Count} productos");
                return Ok(productosDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductoUbicacion] GetAll - Error: {ex.Message}");
                Console.WriteLine($"[ProductoUbicacion] GetAll - StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los productos de una ubicación específica
        /// </summary>
        /// <param name="ubicacionId">ID de la ubicación</param>
        /// <returns>Lista de productos de la ubicación</returns>
        [HttpGet("ubicacion/{ubicacionId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllByUbicacion(Guid ubicacionId, [FromQuery] string? categoria)
        {
            try
            {
                Console.WriteLine($"[ProductoUbicacion] GetAllByUbicacion - Iniciando para ubicación: {ubicacionId}");

                if (ubicacionId == Guid.Empty)
                {
                    Console.WriteLine("[ProductoUbicacion] GetAllByUbicacion - Error: ubicacionId vacío");
                    return BadRequest(new { message = "El ID de ubicación no puede estar vacío" });
                }

                Console.WriteLine("[ProductoUbicacion] GetAllByUbicacion - Llamando al servicio");
                var productosUbicacion = string.IsNullOrEmpty(categoria)
                    ? await _productoUbicacionService.GetAllAsync(ubicacionId)
                    : await _productoUbicacionService.GetAllAsync(ubicacionId, categoria);
                
                Console.WriteLine("[ProductoUbicacion] GetAllByUbicacion - Servicio completado, procesando datos");
                var productos = productosUbicacion.ToList();
                Console.WriteLine($"[ProductoUbicacion] GetAllByUbicacion - Encontrados: {productos.Count} productos");

                // Convertir a DTO para evitar problemas de serialización
                var productosDto = productos.Select(pu => {
                    Console.WriteLine($"[ProductoUbicacion] Procesando producto ID: {pu.Id}");
                    
                    var dto = new {
                        id = pu.Id,
                        ubicacionId = pu.UbicacionId,
                        categoria = pu.Categoria,
                        productoCanjeableId = pu.ProductoCanjeableId,
                        precio = pu.Precio,
                        stockDisponible = pu.StockDisponible,
                        activo = pu.Activo,
                        productoCanjeable = pu.ProductoCanjeable != null ? new {
                            id = pu.ProductoCanjeable.Id,
                            nombre = pu.ProductoCanjeable.Nombre ?? "Sin nombre",
                            descripcion = pu.ProductoCanjeable.Descripcion ?? "Sin descripción",
                            costoEnPuntos = pu.ProductoCanjeable.CostoEnPuntos,
                            fotoUrl = pu.ProductoCanjeable.FotoUrl
                        } : null
                    };
                    
                    Console.WriteLine($"[ProductoUbicacion] Producto procesado: {dto.productoCanjeable?.nombre ?? "SIN PRODUCTO CANJEABLE"}");
                    return dto;
                }).ToList();

                Console.WriteLine($"[ProductoUbicacion] GetAllByUbicacion - DTOs creados: {productosDto.Count}");
                Console.WriteLine("[ProductoUbicacion] GetAllByUbicacion - Retornando resultados");
                
                return Ok(productosDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductoUbicacion] GetAllByUbicacion - ERROR CRÍTICO:");
                Console.WriteLine($"  Mensaje: {ex.Message}");
                Console.WriteLine($"  Tipo: {ex.GetType().Name}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  InnerException: {ex.InnerException.Message}");
                    Console.WriteLine($"  InnerException StackTrace: {ex.InnerException.StackTrace}");
                }

                return StatusCode(500, new { 
                    message = "Error interno del servidor al obtener productos de la ubicación", 
                    details = ex.Message,
                    type = ex.GetType().Name,
                    ubicacionId = ubicacionId.ToString(),
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Endpoint de diagnóstico simple para verificar que el controlador responde
        /// </summary>
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new { 
                mensaje = "Controlador ProductoUbicacion funcionando",
                timestamp = DateTime.UtcNow,
                version = "1.0"
            });
        }

        /// <summary>
        /// Endpoint de diagnóstico para verificar conexión a la base de datos
        /// </summary>
        [HttpGet("diagnostico")]
        public async Task<ActionResult> Diagnostico()
        {
            try
            {
                Console.WriteLine("[ProductoUbicacion] Diagnóstico - Iniciando");

                // Probar obtener todos los productos
                var todosProductos = await _productoUbicacionService.GetAllAsync();
                var listaTodos = todosProductos.ToList();

                var diagnostico = new
                {
                    timestamp = DateTime.UtcNow,
                    mensaje = "Diagnóstico del servicio ProductoUbicacion",
                    totalProductosUbicacion = listaTodos.Count,
                    muestraProductos = listaTodos.Take(3).Select(p => new {
                        id = p.Id,
                        ubicacionId = p.UbicacionId,
                        productoCanjeableId = p.ProductoCanjeableId,
                        stock = p.StockDisponible,
                        activo = p.Activo,
                        tieneProductoCanjeable = p.ProductoCanjeable != null,
                        nombreProducto = p.ProductoCanjeable?.Nombre ?? "NO CARGADO"
                    }).ToList()
                };

                Console.WriteLine($"[ProductoUbicacion] Diagnóstico completado - Total productos: {listaTodos.Count}");
                return Ok(diagnostico);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductoUbicacion] Diagnóstico - Error: {ex.Message}");
                return StatusCode(500, new { 
                    mensaje = "Error en diagnóstico", 
                    error = ex.Message,
                    tipo = ex.GetType().Name,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Verifica si una ubicación existe y tiene productos
        /// </summary>
        [HttpGet("verificar/{ubicacionId}")]
        public async Task<ActionResult> VerificarUbicacion(Guid ubicacionId)
        {
            try
            {
                Console.WriteLine($"[ProductoUbicacion] VerificarUbicacion - Ubicación: {ubicacionId}");

                var productos = await _productoUbicacionService.GetAllAsync(ubicacionId);
                var lista = productos.ToList();

                var resultado = new
                {
                    ubicacionId = ubicacionId,
                    timestamp = DateTime.UtcNow,
                    cantidadProductos = lista.Count,
                    tieneProductos = lista.Count > 0,
                    productosActivos = lista.Count(p => p.Activo),
                    productosConStock = lista.Count(p => p.StockDisponible > 0),
                    productos = lista.Select(p => new {
                        id = p.Id,
                        productoCanjeableId = p.ProductoCanjeableId,
                        stock = p.StockDisponible,
                        activo = p.Activo,
                        tieneProductoCanjeable = p.ProductoCanjeable != null,
                        nombreProducto = p.ProductoCanjeable?.Nombre ?? "SIN NOMBRE",
                        costoEnPuntos = p.ProductoCanjeable?.CostoEnPuntos ?? 0
                    }).ToList()
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductoUbicacion] VerificarUbicacion - Error: {ex.Message}");
                return StatusCode(500, new { 
                    mensaje = "Error al verificar ubicación", 
                    ubicacionId = ubicacionId.ToString(),
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}